$ErrorActionPreference = 'Stop'

$SCRIPT_NAME = "recipe.cake"

Write-Host "Restoring .NET tools"
dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Updating 'includes.cake' with Cake.Recipe content files."
Get-ChildItem "./Source/Cake.Recipe/Content/*.cake" -Exclude "version.cake" | % {
    "#load `"local:?path=$($_.FullName -replace '\\','/')`""
} | Out-File "./includes.cake"

Write-Host "Bootstrapping Cake"
dotnet cake $SCRIPT_NAME --bootstrap
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Running Build"
dotnet cake $SCRIPT_NAME @args
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
