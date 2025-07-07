namespace PhoneAssistant.Model;

public sealed class ApplicationSettings
{
    public int CurrentView { get; set; }

    public bool DarkMode { get; set; }

#if DEBUG
    public string Database { get; set; } = @"c:\dev\paTest.db";
#else
    public string Database { get; set; } = @"\\countyhall.ds2.devon.gov.uk\docs\Exeter, County Hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\phoneassistant.db";
#endif

    public int DefaultDecommissionedTicket { get; set; } = 263323;

    public string DymoPrinter { get; set; } = "Dymo LabelWriter 450";

    public string DymoPrintFile { get; set; } = string.Empty;

    public bool DymoPrintToFile { get; set; } = false;

    public string Printer { get; set; } = @"\\DS2CHL283.ds2.devon.gov.uk\Ricoh-C3503-np04304";

    public string PrintFile { get; set; } = string.Empty;

    public bool PrintToFile { get; set; } = false;

    public string ReleaseUrl { get; set; } = @"\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application";
}
