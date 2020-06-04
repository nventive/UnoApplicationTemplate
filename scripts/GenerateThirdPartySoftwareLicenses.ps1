# GenerateThirdPartySoftwareLicenses
<#
    .SYNOPSIS
        Generates full license information of third-party software used by this application. The output can be saved in a text file and displayed at runtime for attributions purposes.
    .NOTES
        Run the script in the Package Manager Console.
    .EXAMPLE
        .\GenerateThirdPartySoftwareLicenses > ThirdPartySoftwareLicenses.txt
#>

# These packages are not open source and will be filtered out.
$privatePackages = @(
  "Nventive*",
  "Uno.Serialization",  
  "Rx-Single-NoGenerics"
)

# These packages have invalid licenses and will be updated.
$packageLicenses = @{
  "Microsoft.Extensions.Logging*" = "https://github.com/aspnet/Logging/blob/master/LICENSE.txt"
  "Microsoft.SourceLink.Vsts.Git" = "https://github.com/dotnet/sourcelink/blob/master/License.txt"
  "Microsoft.NETCore.UniversalWindowsPlatform" = "https://github.com/Microsoft/dotnet/blob/master/releases/UWP/LICENSE.TXT"
  "*Uno*" = "https://github.com/unoplatform/uno/blob/master/License.md"
  "Xamarin.Android.Support.*" = "https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md"
  "Xamarin.Essentials" = "https://github.com/xamarin/Essentials/blob/develop/LICENSE"
}

# This method will output a list of packages.
function WritePackages 
{
  Param([string]$title, [Object]$packages)

  Write-Output "> $title `n"

  foreach ($package in $packages)
  {
    Write-Output "- $($package.Id) ($($package.LicenseUrl))"
  }
}

Write-Output "Third-Party Software Licenses"
Write-Output "=============================`n"

# Gets all packages for each of the projects in the solution.
$packages = @( 
  Get-Project -All | 
  Where-Object { $_.ProjectName } | 
  ForEach-Object { Get-Package -ProjectName $_.ProjectName } |
  Sort-Object Id -Unique
)

# Turning this on will show all the packages.
if ($FALSE)
{
  WritePackages -title "All libraries" -packages $packages
}

# Some of the packages are not open source, so we filter them.
$privatePackagesMatch = $privatePackages -join "|"
$packages = $packages | Where-Object { $_.Id -NotMatch $privatePackagesMatch }

# Turning this on will show all the open source packages.
if ($FALSE)
{
  WritePackages -title "Open source libraries" -packages $packages
}

# Some of the package licences are not valid, so we update them.
foreach ($key in $packageLicenses.Keys)
{
  foreach ($package in $packages)
  {
    if ($package.Id -like $key)
    {
      $package.LicenseUrl = $packageLicenses[$key] 
    }
  }
}

WritePackages -title "Libraries" -packages $packages
