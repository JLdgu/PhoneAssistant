$newRelease = "K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application\v1.12.11.0"

If (Test-Path -Path $newRelease) {
  Write-Host "Release alread exists"
  Exit
}

Copy-Item -Path "c:\dev\PhoneAssistant\Publish" -Destination $newRelease -Recurse

$newRelease = $newRelease + "\*"
Copy-Item -Path $newRelease -Destination "$ENV:UserProfile\AppData\Local\PhoneAssistant"

# Copy-Item -Path "c:\dev\PhoneAssistant\Publish\*" -Destination "$ENV:UserProfile\AppData\Local\PhoneAssistant"