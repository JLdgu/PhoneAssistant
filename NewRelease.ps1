#Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine

$newRelease = "K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application\v1.405.15"

If (!(Test-Path -Path $newRelease))
{   
    # Copy latest to K:\...
    Copy-Item -Path "c:\dev\PhoneAssistant\Publish" -Destination $newRelease -Recurse
}

# Copy to c:\Users\...
$newRelease = $newRelease + "\*"
New-Item -Path "$ENV:UserProfile\AppData\Local" -Name PhoneAssistant.New -ItemType Directory -ErrorAction SilentlyContinue
Copy-Item -Path $newRelease -Destination "$ENV:UserProfile\AppData\Local\PhoneAssistant.New" -Recurse
