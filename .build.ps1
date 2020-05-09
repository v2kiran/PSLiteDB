
$modulename = "PSlitedb"

task Clean {
    $to = "$PSScriptRoot\module\lib\"
    $debug = "$PSScriptRoot\tests\debug\"
    Get-ChildItem $to | Remove-Item -force
    #Get-ChildItem $debug | Remove-Item -force
}

task Copydll {

    $from = "$PSScriptRoot\src\$modulename\bin\Debug\netstandard2.0\*.dll"
    $to = "$PSScriptRoot\module\lib\"
    $debug = "$PSScriptRoot\tests\debug\"

    Copy-Item $from $to -Force
    #Copy-Item $from $debug -Force
}

task . clean, copydll