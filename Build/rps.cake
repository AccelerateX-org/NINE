using Octopus.Client;
using Octopus.Client.Model;

public static class RPS
{
    private static ICakeContext _context { get; set; }
    private static string _version { get; set; }
    private static BranchDeployment _branchEnvironments { get; set; }
    private static BuildSystem _buildSystem { get; set; }

    public static string BuildVersion 
    { 
        get
        {
            return _version.IsNullOrEmpty() ? BuildParameters.Version.SemVersion : _version;
        }
    }
    
    public static ApiRepository Api { get; private set; }

    public static VcsRepository Vcs { get; private set; }
    
    public static ICollection<GitCommit> GitLog { get; private set; }

    public static OctopusApiClient Octopus { get; private set; }

    public static bool ShouldRunUnitTest { get; set; }

    public static string UatTargetUrl { get; set; }

    public static string UaTestFilePattern { get; private set; }

    public static bool ShouldRunUaTest { get; set; }
    
    public static bool ShouldDeploy { get; private set; }

    public static bool IsDeployed { get; set; }

    public static bool IsMergeBuild 
    { 
        get
        {
            return BuildParameters.IsMasterBranch ? false : System.Text.RegularExpressions.Regex.IsMatch(BuildVersion, @"d+(\.\d+)+([-ci])+\d+");
        } 
    }

    public static string ParseGitLog(NoteFormat format) 
    {
        var log = "";

        if (GitLog == null || GitLog.Count() == 0) {
            return "Changelog: No Changes proccessed";
        }

        if (format == NoteFormat.Plain)
        {
            log += string.Format("Changelog: Last {0} Commit(s)\n\n", GitLog.Count() > 1 ? GitLog.Count().ToString() : "");
            foreach (var item in GitLog)
            {
                log += string.Format("Sha: {0} - {1} ({2}) - {3} - {4} \n", item.Sha, item.Author.Name, item.Author.Email, item.Author.When, item.MessageShort);
            }
        }

        if (format == NoteFormat.Markdown) {
            log += string.Format("### Changelog: Last {0} Commit(s)\n", GitLog.Count() > 1 ? GitLog.Count().ToString() : "");
            foreach (var item in GitLog)
            {
                log += string.Format("* __{0}__ (Commit: [{1}]({2}))\n", item.MessageShort, item.Sha, Vcs.GitHub.CommitUrl + item.Sha);  
                log += string.Format("  * Author: {0} ([{1}](mailto:{1}))\n", item.Author.Name, item.Author.Email);
                log += string.Format("  * Date: {0}\n", item.Author.When);
            }            
        }

        return log;        
    }

    public static string GetDeploymentEnvironment() {
        var IsFeatureBranch = _buildSystem.AppVeyor.Environment.Repository.Branch.StartsWith("feature", StringComparison.OrdinalIgnoreCase);
        var IsSupportBranch = _buildSystem.AppVeyor.Environment.Repository.Branch.StartsWith("support", StringComparison.OrdinalIgnoreCase);
        var env = "Dev";
        
        if (BuildParameters.IsMasterBranch) 
        {
            env = _branchEnvironments.Master;
        }
        if (BuildParameters.IsDevelopBranch) 
        {
            env = _branchEnvironments.Develop;
        }        
        if (IsFeatureBranch) 
        {
            env = _branchEnvironments.Feature;
        }
        if (BuildParameters.IsReleaseBranch) 
        {
            env = _branchEnvironments.Release;
        }        
        if (BuildParameters.IsHotFixBranch) 
        {
            env = _branchEnvironments.Hotfix;
        }   
        if (IsSupportBranch) 
        {
            env = _branchEnvironments.Support;
        }           

        return env;
    }

    public static void Init(
        ICakeContext context,
        BuildSystem buildSystem,
        string repositoryDirectoryPath = "./",
        int repositoryCommitCount = 10,
        string repositoryUrl = null,
        string repositoryCommitUrl = null,
        string buildVersion = null,
        string octopusEndpoint = null,
        string octopusApiKey = null,
        string uaTestFilePattern = null,
        BranchDeployment branchDeployment = null,
        bool shouldDeploy = true)
    {
        if (context == null) 
        {
            throw new ArgumentNullException("Missing context @ RPS.Init()");
        }

        _context = context;
        _buildSystem = buildSystem;
        _version = buildVersion;
        _branchEnvironments  = branchDeployment;

        IsDeployed = false;
        ShouldRunUnitTest = true;
        ShouldRunUaTest = true;

        var gitHubUrlPattern = "https://github.com/{0}/{1}{2}";
        
        VcsProvider githubVcs = new VcsProvider(
            url: repositoryUrl ?? string.Format(gitHubUrlPattern, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, ""),
            commitUrl: repositoryCommitUrl ?? string.Format(gitHubUrlPattern, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, "/commit/")
        );

        ApiCredential octoApi = new ApiCredential
        (
            endpoint: octopusEndpoint ?? context.EnvironmentVariable("OCTO_URL"),
            apiKey: octopusApiKey ?? context.EnvironmentVariable("OCTO_API_KEY")
        );

        if (string.IsNullOrEmpty(octoApi.Endpoint) || 
            string.IsNullOrEmpty(octoApi.ApiKey) ||
            shouldDeploy == false) 
        {
            ShouldDeploy = false;
        }

        Api = new ApiRepository(octopus: octoApi);
        Vcs = new VcsRepository(github: githubVcs);
        
        GitLog = context.GitLog(repositoryDirectoryPath, repositoryCommitCount);

        UaTestFilePattern = uaTestFilePattern;

        if (!BuildParameters.IsLocalBuild && 
            !BuildParameters.IsPullRequest &&
            ShouldDeploy == true)
        {
            Octopus = new OctopusApiClient(octoApi.Endpoint, octoApi.ApiKey);
        }
    }

}

public class VcsRepository
{
    public VcsProvider GitHub { get; private set; }

    public VcsRepository(VcsProvider github) 
    {
        GitHub = github;
    } 
}

public class ApiRepository 
{
    public ApiCredential Octopus { get; private set; }

    public ApiRepository(ApiCredential octopus) 
    {
        Octopus = octopus;
    } 
}

public class VcsProvider 
{
    public string Url { get; private set; }
    public string CommitUrl { get; private set; }

    public VcsProvider(string url, string commitUrl) 
    {
        Url = url;
        CommitUrl = commitUrl;
    } 
}

public class ApiCredential
{
    public string Endpoint { get; private set; }
    public string ApiKey { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }

    public ApiCredential(string endpoint, string apiKey, string userName = null, string password = null)
    {
        Endpoint = endpoint;
        ApiKey = apiKey;
        UserName = userName;
        Password = password;
    }
}

public class OctopusApiClient
{
    private string _endpoint;
    private string _apiKey;
    private OctopusRepository _repository;
    
    public OctopusApiClient(string endpoint, string apiKey) 
    {
        if (endpoint.IsNullOrEmpty() || apiKey.IsNullOrEmpty()) 
        {
            throw new ArgumentNullException("Missing Arguments @ OctopusApiClient()");
        }

        _endpoint = endpoint;
        _apiKey = apiKey;
    }

    public void Connect() {
        _repository = new OctopusRepository(new OctopusServerEndpoint(_endpoint, _apiKey));
    }

    public DeploymentModel GetDeploymentInformation(string deployedVersion) 
    {
        if (deployedVersion.IsNullOrEmpty()) 
        {
            throw new ArgumentNullException("Missing Arguments @ OctopusApiClient.GetDeploymentInformation()");
        }

        if (_repository == null) 
        {
            Connect();
        }

        var release = _repository.Releases.FindOne(r => r.Version.Equals(deployedVersion));

        if (release == null) 
        {
            throw new InvalidOperationException("No releases found or error");
        }

        var project = _repository.Projects.Get(release.ProjectId);
        var channel = _repository.Channels.Get(release.ChannelId);

        if (project == null || channel == null)
        {
            throw new InvalidOperationException("No project/channel found or error");
        }

        var deployment = _repository.Releases.GetDeployments(release).Items.LastOrDefault();
        
        if (deployment == null) 
        {
            throw new InvalidOperationException("No deployment found or error");
        }

        var environment = _repository.Environments.Get(deployment.EnvironmentId);

        if (environment == null) 
        {
            throw new InvalidOperationException("No environment found or error");
        }        

        var task = _repository.Tasks.Get(deployment.TaskId);

        if (task == null) 
        {
            throw new InvalidOperationException("No task found or error");
        }     
               
        var taskDetail = _repository.Tasks.GetDetails(task);

        var machineName = taskDetail.ActivityLogs.LastOrDefault().Children.First(c => c.Name.Equals("Acquire packages", StringComparison.OrdinalIgnoreCase)).Children.Select(c => c.Name).ToArray();

        if (machineName.Count() == 0) {
            throw new InvalidOperationException("No maschine found or error");
        } 
        
        var node = new Uri(_repository.Machines.FindByName(machineName[0]).Uri);

        var target = string.Format("{0}://{1}/{2}/{3}/{4}", node.Scheme, node.Host, project, channel, environment);

        return new DeploymentModel() 
        {
            Project = project.Name,
            Channel = channel.Name,
            Environment = environment.Name,
            Node = node.Host,
            Target = string.Format("{0}://{1}/{2}/{3}/{4}", node.Scheme, node.Host, project.Name, channel.Name, environment.Name),
            Success = task.FinishedSuccessfully
        };
    }
}

public class DeploymentModel 
{
    public string Project { get; set; }
    public string Channel { get; set; }
    public string Environment { get; set; }

    public string Node { get; set; }
    public string Target { get; set; }

    public bool Success { get; set; }
}

public class BranchDeployment
{
    public string Master { get; set; }
    public string Develop { get; set; }
    public string Feature { get; set; }
    public string Release { get; set; }
    public string Hotfix { get; set; }
    public string Support { get; set; }    
}

public enum NoteFormat {
    Plain,
    Markdown
}
