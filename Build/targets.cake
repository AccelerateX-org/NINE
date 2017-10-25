Task("Develop")
    .IsDependentOn("Build")
    .IsDependentOn("Octopus-Packaging")
    .IsDependentOn("Octopus-Deployment");

Task("Integrate")
    .IsDependentOn("Build")
    .IsDependentOn("Test-NUnit")
    .IsDependentOn("Upload-Coveralls")
    .IsDependentOn("Octopus-Packaging")
    .IsDependentOn("Octopus-Deployment");

Task("Approval")
    .IsDependentOn("Integrate")
    .IsDependentOn("Test-NUnit-UAT");    