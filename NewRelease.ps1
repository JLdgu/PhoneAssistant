#Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine

vpk pack -u PhoneAssistant -v 0.408.7 -p .\publish -e PhoneAssistant.exe

Copy-Item -Path "c:\dev\PhoneAssistant\Releases\*" -Destination "K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application" -Recurse -Force

Return

$newRelease = "K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application\v1.406.22"

If (!(Test-Path -Path $newRelease))
{   
    # Copy latest to K:\...
    Copy-Item -Path "c:\dev\PhoneAssistant\Publish" -Destination $newRelease -Recurse
}

# Copy to c:\Users\...
$newRelease = $newRelease + "\*"
New-Item -Path "$ENV:UserProfile\AppData\Local" -Name PhoneAssistant.New -ItemType Directory -ErrorAction SilentlyContinue
Copy-Item -Path $newRelease -Destination "$ENV:UserProfile\AppData\Local\PhoneAssistant.New" -Recurse

Return