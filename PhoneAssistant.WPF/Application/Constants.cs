namespace PhoneAssistant.WPF.Application;

public static class Constants
{
    public const string Email_Main_Boilerplate = """
        <span style="font-size:14px; font-family:Verdana;">
        """;

    public const string Email_Table_Boilerplate = """
        <p><br /></p></span>
        <table style="font-size:12px;font-family: Verdana">
        <tr><th>Order Details</th><th></th></tr>
        """;

    public const string Mobile_Device_DataUsage_Guidance_And_Policy = """
        <p><br /><a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/DCC%20mobile%20phone%20data%20usage%20guidance%20and%20policies.docx?d=w9ce15b2ddbb343739f131311567dd305&csf=1&web=1">
        DCC mobile phone data usage guidance and policies</a></p>
        """;

    public const string Register_With_GovWiFi = """
        <p><br />Before setting up your phone please ensure you register with <a href="https://www.wifi.service.gov.uk/connect-to-govwifi/">GovWifi</a></p>       
        """;

    public const string Setup_Android_Device = """
        <a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx?d=w64bb3f0a09e44557a64bb78311ee513b&csf=1&web=1">
        Android Enterprise - Setting up your Android device (devoncc.sharepoint.com)</a></p>
        """;

    public const string Setup_iOS_Device = """
        <a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/_layouts/15/Doc.aspx?sourcedoc=%7BABC3F4D7-1159-4F72-9C0B-7E155B970A28%7D&file=How%20to%20set%20up%20your%20new%20DCC%20iPhone.docx&action=default&mobileredirect=true">
        Setting up your Apple (iOS) device (devoncc.sharepoint.com)</a></p>
        """;

    public const string Transfer_SIM_Return_Old_Device= """
        <p><br /><span style="color: red;">Don't forget to transfer your old sim to the replacement device</span> before returning the old device to
        DTS End User Compute, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</br>
        You can use <a href="https://www.royalmail.com/track-my-return#/details/5244">Royal Mail Tracked Returns for DCC</a>
            to have the device picked up from your home or you can drop off the item at a Parcel Post Box, Delivery Office or Post Office branch.</p>
        """;

}
