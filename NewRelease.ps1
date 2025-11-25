vpk download local --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"
vpk pack -u PhoneAssistant -v 1.510.30 -p .\publish -e PhoneAssistant.exe -i PhoneAssistant.WPF\Resources\Phone.ico --packAuthors "Devon County Council" --noPortable 
vpk upload local --keepMaxReleases 6 --path "\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application"
