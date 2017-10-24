///////////////////////////////////////////////////////////////////////////////
// Supress breaking the Build
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.DupFinderTask.ContinueOnError();
BuildParameters.Tasks.InspectCodeTask.ContinueOnError();

///////////////////////////////////////////////////////////////////////////////
// Customized Test Task
// Generate reports of processed test results
// Workaround: Reports nevertheless tests are failing
// Source: tools/Cake.Recipe/Content/testing.cake
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestNUnitTask.Task.Actions.Clear();

BuildParameters.Tasks.TestNUnitTask
    .Does(() => RequireTool(NUnitTool, () => {
        EnsureDirectoryExists(BuildParameters.Paths.Directories.NUnitTestResults);

        OpenCover(tool => {
            tool.NUnit3(GetFiles(BuildParameters.Paths.Directories.PublishedNUnitTests + (BuildParameters.TestFilePattern ?? "/**/*Tests.dll")), new NUnit3Settings {
                NoResults = false,
                Work = BuildParameters.Paths.Directories.NUnitTestResults
            });
        },
        BuildParameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
            .WithFilter(ToolSettings.TestCoverageFilter)
            .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
            .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));

        ReportFiles();    
	}))
    .OnError(exception => {
        ReportFiles();

        if(BuildParameters.IsRunningOnAppVeyor)
        {
            AppVeyor.AddMessage("Unit Tests failed!", AppVeyorMessageCategoryType.Error, "Consider Test-Output for further information.");
        }

        throw exception;
    });

// Report Files
public void ReportFiles()
{
    if(BuildParameters.IsRunningOnAppVeyor)
    {
        // Upload results to AppVeyor.
        AppVeyor.UploadTestResults(BuildParameters.Paths.Directories.NUnitTestResults + "/TestResult.xml", AppVeyorTestResultsType.NUnit3);
    }    
    
    var reportSubDir = "/Report";
    var unitTestZip = BuildParameters.Paths.Directories.NUnitTestResults.CombineWithFilePath("UnitTestReport.zip");
    var codeCoverageZip = BuildParameters.Paths.Directories.TestCoverage.CombineWithFilePath("CodeCoverageReport.zip");

    // Generate Unit Test Report
    var NUnitTestReport = BuildParameters.Paths.Directories.NUnitTestResults + reportSubDir;
    EnsureDirectoryExists(NUnitTestReport);
    ReportUnit(BuildParameters.Paths.Directories.NUnitTestResults, NUnitTestReport, new ReportUnitSettings());

    // Generate Code Coverage Report
    var codeCoverageReport = BuildParameters.Paths.Directories.TestCoverage + reportSubDir;
    ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, codeCoverageReport);
  
    Information("Zipping Unit Test Results...");
    Zip(NUnitTestReport, unitTestZip);

    Information("Zipping Code Coverage Results...");
    Zip(codeCoverageReport, codeCoverageZip);

    // Upload to AppVeyor if Build System
    if (BuildParameters.IsRunningOnAppVeyor) 
    {
        if (FileExists(unitTestZip)) 
        {
            AppVeyor.UploadArtifact(unitTestZip);
        }
        if (FileExists(codeCoverageZip)) 
        {
            AppVeyor.UploadArtifact(codeCoverageZip);
        }
     }    
}

///////////////////////////////////////////////////////////////////////////////
// Customized Coveralls Task
// Upload reports even on PR
// See: tools/Cake.Recipe/Content/coveralls.cake
///////////////////////////////////////////////////////////////////////////////

Task("Upload-Coveralls")
    .WithCriteria(() => FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .Does(() => RequireTool(CoverallsTool, () => {
        if(BuildParameters.CanPublishToCoveralls)
        {
            CoverallsIo(BuildParameters.Paths.Files.TestCoverageOutputFilePath, new CoverallsIoSettings()
            {
                RepoToken = BuildParameters.Coveralls.RepoToken
            });
        }
        else
        {
            Warning("Unable to publish to Coveralls, as necessary credentials are not available");
        }
    }))
    .OnError(exception =>
    {
        Error(exception.Message);
        Information("Upload-Coveralls-Report Task failed, but continuing with next Task...");
        publishingError = true;
    });
 