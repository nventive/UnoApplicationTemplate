# This script takes the APP_README template and generates a README.md file with the correct version number and commit information.

param (
    [string]$InputPath, # The path to the APP_README.md file.
    [string]$OutputPath = $InputPath, # This is an option because you might want to generate the output with the README.md name.
    [string]$VersionNumber,
    [string]$CommitShortSha,
    [string]$CommitFullSha,
    [string]$CommitDate
)

$content = Get-Content -Path $InputPath

$content = $content -replace "{{app-template-version}}", $VersionNumber 
$content = $content -replace "{{app-template-commit-short-sha}}", $CommitShortSha
$content = $content -replace "{{app-template-commit-full-sha}}", $CommitFullSha
$content = $content -replace "{{app-template-commit-date}}", $CommitDate

$content | Out-File -FilePath $OutputPath -Encoding utf8
