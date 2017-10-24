///////////////////////////////////////////////////////////////////////////////
// Octopus Deploy - Package via MSBuild Task
///////////////////////////////////////////////////////////////////////////////


Task("Pre-Package-CleanUp")
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.Does(() =>
	{
		Information("Cleaning...");
    	
		// TODO: GetOutputAssemblies(new FilePath("test.sln"), "Release");

		var solution = ParseSolution(BuildParameters.SolutionFilePath);
		var projects = solution.Projects;
		
		foreach(var project in projects) {
		
            // If no Solution Folder
			if (project.Type != "{2150E333-8FDC-42A3-9474-1A3956D46DE8}") {
                
            Information("Cleaning {0}", project.Path);
                
            var dir = project.Path.GetDirectory();
            
			CleanDirectories(dir + "/bin");
            CleanDirectories(dir + "/obj");
		
		    }
	    }
	}
);


Task("Generate-Changelog")
	.IsDependentOn("Clean")
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.Does(() =>
	{
		EnsureDirectoryExists(BuildParameters.Paths.Directories.Build);

		var plainChangelog = BuildParameters.Paths.Directories.Build.ToString() + "/Changelog.txt";
		var mdChangelog = BuildParameters.Paths.Directories.Build.ToString() + "/Changelog.md";
		
		System.IO.File.Create(plainChangelog).Dispose();
		System.IO.File.WriteAllText(plainChangelog, RPS.ParseGitLog(format: NoteFormat.Plain));
		
		System.IO.File.Create(mdChangelog).Dispose();
		System.IO.File.WriteAllText(mdChangelog, RPS.ParseGitLog(format: NoteFormat.Markdown));
	}
);


Task("Build-Package")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Pre-Package-CleanUp")
	.IsDependentOn("Generate-Changelog")
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.Does(() => 
	{
		var msbuildSettings = new MSBuildSettings()
		        .SetPlatformTarget(ToolSettings.BuildPlatformTarget)
                .UseToolVersion(ToolSettings.BuildMSBuildToolVersion)
				.SetMaxCpuCount(ToolSettings.MaxCpuCount)
                .SetConfiguration(BuildParameters.Configuration)
                .WithTarget("Build")
				.WithProperty("RunOctoPack", "true")
				.WithProperty("OctoPackPackageVersion", RPS.BuildVersion)
				.WithProperty("OctoPackReleaseNotesFile", MakeAbsolute(BuildParameters.Paths.Directories.Build) + "/Changelog.txt")
				.WithProperty("OctoPackPublishPackageToFileShare", MakeAbsolute(BuildParameters.Paths.Directories.PublishedApplications).ToString());

		MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);	

		if (BuildParameters.IsRunningOnAppVeyor) 
		{
			foreach(var package in GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/*.nupkg"))
    		{
        		AppVeyor.UploadArtifact(package);
    		}	
		}
	}
);


///////////////////////////////////////////////////////////////////////////////
// Target Definiton
///////////////////////////////////////////////////////////////////////////////

Task("Octopus-Packaging")
	.WithCriteria(() => !BuildParameters.IsPullRequest)
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Pre-Package-CleanUp")
	.IsDependentOn("Generate-Changelog")
    .IsDependentOn("Build-Package");
