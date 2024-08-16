# Copy Live PhoneAssistant db to c:/temp
$live = "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\PhoneAssistant.db"
$test =  "c:/dev/paTest.db" 

If (Test-Path -Path $test) {
  Remove-Item $test
  Write-Host "Deleted $test"
}

Copy-Item $live $test

dotnet run --project DbUtil test