///////////////////////////////////////////////////////////////////////////////
// Octopus Deploy - Deployment
///////////////////////////////////////////////////////////////////////////////


Task("Push-To-Package-Feed")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.IsDependentOn("Build-Package")
	.Does(() => 
		{
			OctoPush(RPS.Api.Octopus.Endpoint, RPS.Api.Octopus.ApiKey, GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/*.nupkg"),
				new OctopusPushSettings {
					ReplaceExisting = true
				}
			);
		}
	)
	.OnError(exception =>
		{
			Error(exception.Message);
			Information("Push-To-Package-Feed Task failed, but continuing with next Task...");
			
			if(BuildParameters.IsRunningOnAppVeyor)
			{
				AppVeyor.AddMessage("Pushing to Package Feed failed!", AppVeyorMessageCategoryType.Warning, "Please manually upload Artefact to Octopus Deploy");
			}
			
			throw exception;
		}
	);

Task("Create-Release-From-Package")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.IsDependentOn("Push-To-Package-Feed")
	.Does(() => 
		{
			OctoCreateRelease(BuildParameters.RepositoryName, new CreateReleaseSettings 
			{
				Server = RPS.Api.Octopus.Endpoint,
				ApiKey = RPS.Api.Octopus.ApiKey,
				ReleaseNumber = RPS.BuildVersion,
				ReleaseNotesFile = MakeAbsolute(BuildParameters.Paths.Directories.Build) + "/Changelog.md",
				Packages = new Dictionary<string, string>
				{
					{ 
						BuildParameters.RepositoryName, RPS.BuildVersion
					}
				},
			});
		}	
	)
	.OnError(exception =>
		{
			Error(exception.Message);
			Information("Create-Release-From-Package Task failed, but continuing with next Task...");
			
			if(BuildParameters.IsRunningOnAppVeyor)
			{
				AppVeyor.AddMessage("Creating a Release from Package failed!", AppVeyorMessageCategoryType.Warning, "Please manually create a Release on Octopus Deploy");
			}
			
			throw exception;
		}
	);

Task("Deploy-Package")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.IsDependentOn("Push-To-Package-Feed")
	.IsDependentOn("Create-Release-From-Package")
	.Does(() => 
		{
			OctoDeployRelease
			(
				RPS.Api.Octopus.Endpoint, 
				RPS.Api.Octopus.ApiKey, 
				BuildParameters.RepositoryName, 
				RPS.GetDeploymentEnvironment(),
				RPS.BuildVersion, 
				new OctopusDeployReleaseDeploymentSettings 
				{
					ShowProgress = true
				}
			);
			
			var client = RPS.Octopus;
			client.Connect();
			
			//Set Deployment Target as BaseURL for UAT
			RPS.UatTargetUrl = client.GetDeploymentInformation(RPS.BuildVersion).Target;

			Information("Target: " + RPS.UatTargetUrl);

		}	
	)
	.OnError(exception =>
		{
			Error(exception.Message);
			Information("Deploy-Package Task failed, but continuing with next Task...");
			
			if(BuildParameters.IsRunningOnAppVeyor)
			{
				AppVeyor.AddMessage("Deployment of Package failed!", AppVeyorMessageCategoryType.Warning, "Please manually deploy the Release on Octopus Deploy");
			}
			
			throw exception;
		}
	);	


///////////////////////////////////////////////////////////////////////////////
// Target Definiton
///////////////////////////////////////////////////////////////////////////////


Task("Octopus-Deployment")
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .IsDependentOn("Push-To-Package-Feed")
	.IsDependentOn("Create-Release-From-Package")
	.IsDependentOn("Deploy-Package");
