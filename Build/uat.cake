Task("Test-NUnit-UAT")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.WithCriteria(() => !BuildParameters.IsPullRequest)
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedNUnitTests))
    .WithCriteria(() => RPS.IsDeployed)
    .WithCriteria(() => RPS.ShouldRunUaTest)
    .IsDependentOn("Octopus-Deployment")
	.Does(() => RequireTool(NUnitTool, () => 
		{
            var uatResults = BuildParameters.Paths.Directories.NUnitTestResults + "/UAT";
            EnsureDirectoryExists(uatResults);

            NUnit3(GetFiles(BuildParameters.Paths.Directories.PublishedNUnitTests + (RPS.UaTestFilePattern ?? "/**/*.UaTests.dll")), new NUnit3Settings {
                NoResults = false,
                Work = uatResults,
                EnvironmentVariables = new Dictionary<string, string> 
                {
                    { "SL_BASE_URL", RPS.UatTargetUrl },
                    { "SL_BUILD", RPS.BuildVersion }
                }
            });

           if(BuildParameters.IsRunningOnAppVeyor)
            {
                // Upload results to AppVeyor.
                AppVeyor.UploadTestResults(uatResults + "/TestResult.xml", AppVeyorTestResultsType.NUnit3);
            }   
        }))
        .OnError(exception => {
        
            if(BuildParameters.IsRunningOnAppVeyor)
            {
                // Upload results to AppVeyor.
                AppVeyor.UploadTestResults(BuildParameters.Paths.Directories.NUnitTestResults + "/UAT/TestResult.xml", AppVeyorTestResultsType.NUnit3);
                AppVeyor.AddMessage("User Accaptance Tests failed!", AppVeyorMessageCategoryType.Error, "Consider Test-Output for further information.");
            }
           
            throw exception;

        });