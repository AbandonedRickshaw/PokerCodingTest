Param (
	[ValidateSet(".\poker\Tests\")]
	[Parameter(Mandatory=$true)][string] $path
)

Get-ChildItem $path -Filter *.tst | 

Foreach-Object {
    $content = Get-Content $_.FullName
	[System.Collections.ArrayList]$rows = $content
	$rows.Remove($content[-1])
	$output = .\poker\bin\Debug\poker.exe $rows

	#Write-Output $output


	Write-Output $_.Name
	If ($content[-1] -eq $output) {
		Write-Host "Success" -ForegroundColor Green
	}  Else {
		Write-Host "Failed" -ForegroundColor Red
	} 

}