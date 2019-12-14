param
(

    [Parameter(Mandatory=$false)]
    [string] $NuGetApiKey

)


$ErrorActionPreference = "Stop";
Set-StrictMode -Version "Latest";


$thisScript = $MyInvocation.MyCommand.Path;
$thisFolder = [System.IO.Path]::GetDirectoryName($thisScript);
$rootFolder = [System.IO.Path]::GetDirectoryName($thisFolder);
write-host "this script = '$thisScript'";
write-host "this folder = '$thisFolder'";
write-host "root folder = '$rootFolder'";


# import all library functions
write-host "dot-sourcing script files";
$libFolder = [System.IO.Path]::Combine($thisFolder, "lib");
$filenames = [System.IO.Directory]::GetFiles($libFolder, "*.ps1");
foreach( $filename in $filenames )
{
    write-host "    $filename";
    . $filename;
}


Set-PowerShellHostWidth -Width 500;


$vsRoot       = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools";
$msbuild      = "$vsRoot\MSBuild\Current\Bin\MSBuild.exe";
$solution     = [System.IO.Path]::Combine($rootFolder, "src\Kingsland.ParseFx.sln");

$pkgFolder    = [System.IO.Path]::Combine($rootFolder, "packages");
$gitVersion   = [System.IO.Path]::Combine($pkgFolder, "gitversion.commandline\5.1.2\tools\GitVersion.exe");
$nunitConsole = [System.IO.Path]::Combine($pkgFolder, "nunit.consolerunner\3.9.0\tools\nunit3-console.exe");
$nuget        = [System.IO.Path]::Combine($pkgFolder, "nuget.commandline\5.3.1\tools\NuGet.exe");

$nunitTestAssemblies = @(
#    [System.IO.Path]::Combine($rootFolder, "src\Kingsland.MofParser.UnitTests\bin\Debug\Kingsland.MofParser.UnitTests.dll")
);

$nuspec = [System.IO.Path]::Combine($rootFolder, "src\Kingsland.ParseFx\Kingsland.ParseFx.nuspec");


if( Test-IsTeamCityBuild )
{

    # read the build properties
    $properties = Read-TeamCityBuildProperties;
    $NuGetApiKey = $properties.NuGetApiKey;

    # copy teamcity addins for nunit into build folder
    Install-TeamCityNUnitAddIn -TeamCityNUnitAddin $properties["system.teamcity.dotnet.nunitaddin"] `
                               -NUnitRunnersFolder $nunitRunners;

}


# restore nuget packages for the solution
$env:NUGET_PACKAGES         = $pkgFolder;
$env:NUGET_HTTP_CACHE_PATH  = $pkgFolder;
$msbuildParameters = @{
    "MsBuildExe"   = $msbuild
    "Solution"     = $solution
    "Targets"      = @( "Restore" )
    "Properties"   = @{}
    #"Verbosity"    =  "minimal"
    "Verbosity"    =  "normal"
};
Invoke-MsBuild @msbuildParameters;


# determine build number for the nuget package
$versionInfo = Invoke-GitVersion -GitVersion $gitVersion;
#$buildNumber = $versionInfo.SemVer;
$buildNumber = $versionInfo.LegacySemVer;
write-host "version info = ";
write-host ($versionInfo | fl * | out-string);
write-host "build number = '$buildNumber'";


# build the solution
$env:NUGET_PACKAGES         = $pkgFolder;
$env:NUGET_HTTP_CACHE_PATH  = $pkgFolder;
$msbuildParameters = @{
    "MsBuildExe"   = $msbuild
    "Solution"     = $solution
    "Targets"      = @( "Clean", "Restore", "Build" )
    "Properties"   = @{
        "PackageVersion" = $BuildNumber
    }
    "Verbosity"    =  "normal"
};
Invoke-MsBuild @msbuildParameters;


# execute unit tests
foreach( $assembly in $nunitTestAssemblies )
{
    Invoke-NUnitConsole -NUnitConsole $nunitConsole -Assembly $assembly;
}


# package the solution
$env:NUGET_PACKAGES         = $pkgFolder;
$env:NUGET_HTTP_CACHE_PATH  = $pkgFolder;
$msbuildParameters = @{
    "MsBuildExe"   = $msbuild
    "Solution"     = $solution
    "Targets"      = @( "Pack" )
    "Properties"   = @{
        "PackageVersion" = $BuildNumber
    }
    "Verbosity"    =  "normal"
};
Invoke-MsBuild @msbuildParameters;


# push nuget package
if( $PSBoundParameters.ContainsKey("NuGetApiKey") )
{
    $nupkg = [System.IO.Path]::Combine($rootFolder, "src\Kingsland.ParseFx\bin\Debug\Kingsland.ParseFx.$BuildNumber.nupkg");
    Invoke-NuGetPush -NuGet $nuget -PackagePath $nupkg -Source "https://nuget.org" -ApiKey $NuGetApiKey;
}
