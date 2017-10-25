#load "nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease&version=0.3.0-unstable0278"

#addin "nuget:?package=Octopus.Client&version=4.22.1"
#tool "nuget:?package=OctopusTools&version=4.22.1"

#load "./Build/rps.cake"
#load "./Build/targets.cake"
#load "./Build/adjustments.cake"
#load "./Build/package.cake"
#load "./Build/deployment.cake"
#load "./Build/uat.cake"

Information(Figlet("RPS"));
Warning("Rocket Production System");


Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./",
                            integrationTestScriptPath: ".", // Workaround: NULL Exception
                            testFilePattern: "/**/*.Tests.dll",
                            title: "MyStik.TimeTable",
                            repositoryOwner: "AccelerateX-org",
                            repositoryName: "NINE",
                            appVeyorAccountName: "AccelerateX",
                            shouldExecuteGitLink: false,
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false);
                            
BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[WIQuest*]* -[WIQuest*.Tests]* -[WIQuest*.UaTests]*");

RPS.Init(context: Context,
        buildSystem: BuildSystem,
        uaTestFilePattern: "/**/*.UaTests.dll",
        shouldRunUnitTest: true,
        shouldRunUaTest: true,
        shouldDeploy: true,
        branchDeployment: new BranchDeployment() 
        {
            Tag = "Staging",
            Master = "Staging",
            Develop = "Dev",
            Feature = "Dev",
            Release = "QA",
            Hotfix = "Staging",
            Support = "Staging"
        });

if (BuildParameters.IsDevelopBranch && !BuildParameters.IsPullRequest) 
{
    RPS.ShouldRunUnitTest = true;
}

Build.Run();