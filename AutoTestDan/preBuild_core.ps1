param([string]$configurationName)

write-host "Configuration: $configurationName"

$scriptFile = $MyInvocation.MyCommand.Source;
if(!$scriptFile) {
	#earlier powershell versions used path instead of source, so try that
	$scriptFile = $MyInvocation.MyCommand.Path;
}
if(!$scriptFile) {
	write-host "can't determine path of this script so abort now";
	exit(999);
}

$scriptPath = Split-Path $scriptFile;

$batFolder = Join-Path $scriptPath "bin\$configurationName\Files";


New-Item -ItemType directory -Path $batFolder -ErrorAction Ignore;

$batSrc = Join-Path $scriptPath "..\Files\";


if($batSrc -And $batFolder) {
	write-host "do copy"
	cp "$batSrc\*" $batFolder;
} else {
	write-host "can't figure out which folders to copy so abort now";
	exit(999);
}

write-host "show results"
ls $batFolder
