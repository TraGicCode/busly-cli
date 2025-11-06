var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var srcDirectory = "./src";
var consoleCsproj = $"{srcDirectory}/NServiceBusCLI.Console/NServiceBusCLI.Console.csproj";
var testsDirectory = "./tests";
var artifactsDirectory = "./artifacts";
var solution = $"./NServiceBusCLI.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectories($"./{srcDirectory}/**/bin");
    CleanDirectories($"./{srcDirectory}/**/obj");
    CleanDirectories($"./{testsDirectory}/**/bin");
    CleanDirectories($"./{testsDirectory}/**/obj");
    CleanDirectory(artifactsDirectory);
});

Task("Compile")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild(solution, new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Compile")
    .Does(() =>
{
    DotNetTest(solution, new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    });
});

Task("Format-Check")
    .Does(() =>
{
    var settings = new DotNetFormatSettings
    {
        NoRestore = true,
        VerifyNoChanges = true
    };

    DotNetFormat(".", settings);
});

Task("Format-Fix")
    .Does(() =>
{
    var settings = new DotNetFormatSettings
    {
        NoRestore = true,
        VerifyNoChanges = false
    };

    DotNetFormat(".", settings);
});

Task("Pack-DotNetTool")
    .Does(() =>
{
    DotNetPack(consoleCsproj, new DotNetPackSettings
    {
        Configuration = configuration,
        OutputDirectory = "./artifacts/nupkgs",
    });
});


//////////////////////////////////////////////////////////////////////
// Targets
//////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Format-Check")
    .IsDependentOn("Clean")
    .IsDependentOn("Compile")
    .IsDependentOn("Test");


Task("PackDotNetTool")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);