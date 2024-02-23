# Copy Live PhoneAssistant db to c:/temp
$live = "K:/FITProject/ICTS/Mobile Phones/PhoneAssistant/PhoneAssistant.db"
$test =  "c:/temp/paTest.db" 

If (Test-Path -Path $test) {
  Remove-Item $test
  Write-Host "Deleted $test"
}

Copy-Item $live $test