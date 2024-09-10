#Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope LocalMachine

vpk download local --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"
vpk pack -c beta -u PhoneAssistant -v 0.409.9 -p .\publish -e PhoneAssistant.exe -i PhoneAssistant.WPF\Resources\Phone.ico --packAuthors "Devon County Council" --noPortable 

Return
vpk upload local -c beta --keepMaxReleases 6 --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"
