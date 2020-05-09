[CmdletBinding()]
param(
  [switch]
  $Bootstrap,

  [switch]
  $Compile,

  [switch]
  $Test
)

# Bootstrap step
if ($Bootstrap.IsPresent)
{
  Write-Information "Validate and install missing prerequisits for building ..."

  # For testing Pester
  if (-not (Get-Module -Name Pester -ListAvailable) -or (Get-Module -Name Pester -ListAvailable)[0].Version -eq [Version]'3.4.0')
  {
    Write-Warning "Module 'Pester' is missing. Installing 'Pester' ..."
    Install-Module -Name Pester -Scope CurrentUser -Force
  }

}

# Compile step
if ($Compile.IsPresent)
{
  if (Get-Module PSLiteDB)
  {
    Remove-Module PSLiteDB -Force
  }

  if ((Test-Path ./Output))
  {
    Remove-Item -Path ./Output -Recurse -Force
  }

  # Copy non-script files to output folder
  if (-not (Test-Path .\Output))
  {
    $null = New-Item -Path .\Output -ItemType Directory
  }

  Copy-Item -Path '.\module\*' -Filter '*.*'  -Recurse -Destination .\Output -Force


  # Copy Module README file
  Copy-Item -Path '.\README.md' -Destination .\Output -Force


  Rename-Item -Path .\Output -NewName 'PSLiteDB'

  # Compress output, for GitHub release
  Compress-Archive -Path .\PSLiteDB\* -DestinationPath .\AzurePipelines\PSLiteDB.zip


}

# Test step
if ($Test.IsPresent)
{
  if (-not (Get-Module -Name Pester -ListAvailable))
  {
    throw "Cannot find the 'Pester' module. Please specify '-Bootstrap' to install build dependencies."
  }

  if ($env:TF_BUILD)
  {
    $res = Invoke-Pester "./Tests" -OutputFormat NUnitXml -OutputFile TestResults.xml -PassThru
    if ($res.FailedCount -gt 0) { throw "$($res.FailedCount) tests failed." }
  }

}