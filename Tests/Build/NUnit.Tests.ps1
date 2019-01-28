$include = "*.Tests.dll", "*.Tests.NUnit.dll"
$exclude = "ExcludeTests"

$wc = New-Object System.Net.WebClient

foreach ($n in ("45","40","35","30","20")) {
    $logFileName = "$env:APPVEYOR_BUILD_FOLDER\_Results\net_" + $n + "nunit_results.xml"
    echo "nunit3-console Tests\bin\Debug\" $n "\Tests.dll --result=$logFileName"
    &"nunit3-console Tests\bin\Debug\" $n "\Tests.dll --result=$logFileName"
    if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
    echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
    $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID", "$logFileName")
    if ($LastExitCode -ne 0) {
        echo "FAIL: UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
        $host.SetShouldExit($LastExitCode)
}

#run .net core tests
#$a = (gci -include $include -r | `
#	where { $_.fullname -match "\\bin\\Release\\netcore" -and $_.fullname -notmatch $exclude } | `
#	select -ExpandProperty FullName)
#
#$logFileName = "$env:APPVEYOR_BUILD_FOLDER\_Results\netcore_nunit_results.xml"
#echo "dotnet vstest $a --logger:'trx;LogFileName=$logFileName'"
#dotnet vstest $a --logger:"trx;LogFileName=$logFileName"
#if ($LastExitCode -ne 0) {
#	echo "FAIL: dotnet vstest $a --logger:'trx;LogFileName=$logFileName'"
#	$host.SetShouldExit($LastExitCode)
#}
#echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
#$wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID", "$logFileName")
#if ($LastExitCode -ne 0) {
#	echo "FAIL: UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
#	$host.SetShouldExit($LastExitCode)
#}
#