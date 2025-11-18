var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var version = Argument("package-version", "0.1.0"); // default version if none is passed

var srcDirectory = "./src";
var consoleCsproj = $"{srcDirectory}/BuslyCLI.Console/BuslyCLI.Console.csproj";
var testsDirectory = "./tests";
var artifactsDirectory = "./artifacts";
var solution = $"./BuslyCLI.sln";

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
    var msBuildSettings = new DotNetMSBuildSettings()
      .WithProperty("InformationalVersion", version);
    DotNetBuild(solution, new DotNetBuildSettings
    {
        Configuration = configuration,
        MSBuildSettings = msBuildSettings,
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
        NoRestore = true,
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
    .IsDependentOn("Compile")
    .Does(() =>
{
    DotNetPack(consoleCsproj, new DotNetPackSettings
    {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = "./artifacts/nupkgs",
        MSBuildSettings = new DotNetMSBuildSettings
        {
            Version = version,
        },
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