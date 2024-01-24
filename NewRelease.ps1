$newRelease = "K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application\v1.401.24.0"

If (Test-Path -Path $newRelease)
{    
    $newRelease = $newRelease + "\*"
    New-Item -Path "$ENV:UserProfile\AppData\Local" -Name PhoneAssistant.New -ItemType Directory
    Copy-Item -Path $newRelease -Destination "$ENV:UserProfile\AppData\Local\PhoneAssistant.New"
}
else
{
    Copy-Item -Path "c:\dev\PhoneAssistant\Publish" -Destination $newRelease -Recurse
}