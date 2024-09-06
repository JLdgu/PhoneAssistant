#Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine

vpk pack -u PhoneAssistant -v 0.408.23 -p .\publish -e PhoneAssistant.exe -i PhoneAssistant.WPF\Resources\Phone.ico --packAuthors "Devon County Council" --noPortable 

#Copy-Item -Path "c:\dev\PhoneAssistant\Releases\*" -Destination "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application" -Recurse -Force
vpk upload local --keepMaxReleases 6 --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"
Return
