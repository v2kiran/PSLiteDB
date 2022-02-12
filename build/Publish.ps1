param  (
    [string] $PSGalleryApiKey
)

# Purpose: Cleans the module directory to prepare for publication to PowerShell Gallery
$ErrorActionPreference = 'stop'
$ModulePath = "$PSScriptRoot/../module" | convert-path


# Build directory should work on Windows or Linux
$BuildDirectory = $env:TEMP ? "$env:TEMP/PSLiteDB" : '/tmp/PSLiteDB'

# These project paths will be excluded from the module during publishing
$Exclude = @(
    'README.md'
    'tests'
    '.github'
    'BuildNumber'
)

# Copy project to temporary build directory
if (Test-Path -Path $BuildDirectory)
{
    Remove-Item -Path $BuildDirectory -Recurse -Force
}
$null = New-Item -ItemType Directory -Path $BuildDirectory
Write-Host -Object 'Created build directory'
Copy-Item -Exclude $Exclude -Path $ModulePath/* -Destination $BuildDirectory -Recurse
Write-Host -Object 'Copied all items to build directory'

# Replace module version with build number
$ManifestPath = "$BuildDirectory/PSLiteDB.psd1"
#(Get-Content -Path $ManifestPath -Raw) -replace 'ModuleVersion = ''(?<version>.*?)''', ('ModuleVersion = ''${{version}}.{0}''' -f (Get-Content -Path $ModulePath/BuildNumber)) | Set-Content -Path $ManifestPath

Publish-Module -Path $BuildDirectory -NuGetApiKey $PSGalleryApiKey