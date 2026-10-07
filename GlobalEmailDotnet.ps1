<#
.SYNOPSIS
    Builds and runs the Melissa Global Email Cloud API .NET sample.

.DESCRIPTION
    This script builds GlobalEmailDotnet with dotnet publish, then runs the
    resulting executable, passing along the license and (if supplied) the email.

    Overall flow:
      1. Resolve the license (parameter, prompt, or MD_LICENSE environment variable).
      2. Publish GlobalEmailDotnet in Release configuration to
         .\GlobalEmailDotnet\Build.
      3. Run the built executable: one-shot mode if -email was supplied,
         otherwise interactive mode (the .NET program prompts for the email).

.PARAMETER email
    Email address to verify in one-shot mode.

.PARAMETER license
    License string. Resolved in this order:
      1. This parameter.
      2. An interactive prompt, if the parameter was not supplied.
      3. The MD_LICENSE environment variable, if the prompt was left blank.
    Note that the environment variable is the last resort, not the first: running
    without -license always prompts, even when MD_LICENSE is set.

.PARAMETER quiet
    Accepted for parity with other sample scripts; not currently used to suppress output.

.EXAMPLE
    .\GlobalEmailDotnet.ps1 -license "your-license"

.EXAMPLE
    .\GlobalEmailDotnet.ps1 -email "Info@melissa.com" -license "your-license"
#>

######################### Parameters ##########################
param(
    $email = '',
    $license = '',
    [switch]$quiet = $false
    )

# Uses the location of the .ps1 file
$CurrentPath = $PSScriptRoot
Set-Location $CurrentPath
$ProjectPath = "$CurrentPath\GlobalEmailDotnet"
$BuildPath = "$ProjectPath\Build"

If (!(Test-Path $BuildPath)) {
  New-Item -Path $ProjectPath -Name 'Build' -ItemType "directory"
}

########################## Main ############################
Write-Host "`n================== Melissa Global Email Cloud API ====================`n"

# Get license (either from parameters or user input)
if ([string]::IsNullOrEmpty($license) ) {
  $license = Read-Host "Please enter your license string"
}

# Check for License from Environment Variables 
if ([string]::IsNullOrEmpty($license) ) {
  $license = $env:MD_LICENSE
}

if ([string]::IsNullOrEmpty($license)) {
  Write-Host "`nLicense String is invalid!"
  Exit
}

# Start program
# Build project
Write-Host "`n============================ BUILD PROJECT ==========================="

dotnet publish -f="net7.0" -c Release -o $BuildPath GlobalEmailDotnet\GlobalEmailDotnet.csproj

# Run project
# No email supplied -> run interactively; otherwise pass it through for one-shot mode.
if ([string]::IsNullOrEmpty($email)) {
  dotnet $BuildPath\GlobalEmailDotnet.dll --license $license 
}
else {
  dotnet $BuildPath\GlobalEmailDotnet.dll --license $license --email $email
}
