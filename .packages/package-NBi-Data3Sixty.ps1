param(
    [parameter(Mandatory=$true)]
    [string]$version
)

$root = (split-path -parent $MyInvocation.MyCommand.Definition)

Write-Host "Calculating dependencies ..."

$dependencies = @{}
$solutionRoot = Join-Path ($root) ".."
Write-Host "Looking for projects in $solutionRoot ..."
$projects = Get-ChildItem $solutionRoot | ?{ $_.PSIsContainer -and $_.Name -like "*Data3Sixty*"-and $_.Name -notLike "*Testing*"} | select Name, FullName
foreach($proj in $projects)
{
    $projName = $proj.name
    Write-Host "Looking for dependencies in project $projName ..."
    $path = Join-Path ($proj.FullName) "packages.config"
        
    if(Test-Path $path)
    {
        [xml]$packages = Get-Content $path
        foreach($package in $packages.FirstChild.NextSibling.ChildNodes)
        {
            if (!$dependencies.ContainsKey($package.id) -and ($package.id.StartsWith("NBi") -or  ($package.id.StartsWith("NUnit") -and !$package.id.EndsWith("NUnit")) )) 
            {
                $dependencies.add($package.id, "<dependency id=""$($package.id)"" version=""$(($package.allowedVersions, $package.version -ne $null)[0])"" />")
            }
        }
    }
}

Write-Host "Found $($dependencies.Count) dependencies ..."
$depList = $dependencies.Values -join [Environment]::NewLine + "`t`t"

#For NBi.Data3Sixty (dll)
$lib = "$root\NBi.Data3Sixty\lib\net462\"
If (Test-Path $lib)
{
	Remove-Item $lib -recurse
}
new-item -Path $lib -ItemType directory
new-item -Path $root\..\.nupkg -ItemType directory -force
Copy-Item $root\..\NBi.Data3Sixty\bin\Debug\NBi.*Data3Sixty*.dll $lib

Write-Host "Setting .nuspec version tag to $version"

$content = (Get-Content $root\NBi.Data3Sixty\NBi.Data3Sixty.nuspec -Encoding UTF8) 
$content = $content -replace '\$version\$',$version
$content = $content -replace '\$depList\$',$depList

$content | Out-File $root\NBi.Data3Sixty\NBi.Data3Sixty.compiled.nuspec -Encoding UTF8

& NuGet.exe pack $root\..\.packages\NBi.Data3Sixty\NBi.Data3Sixty.compiled.nuspec -Version $version -OutputDirectory $root\..\.nupkg
