namespace Reconcile;
public class Device
{
    public string Name { get; }
    public string AssetTag { get; }
    public string SerialNumber { get; }
    public string Status { get; }

    public Device(string name, string assetTag, string serialNumber, string status)
    {
        Name = name;
        AssetTag = assetTag;
        SerialNumber = serialNumber;
        Status = status;
    }
}
