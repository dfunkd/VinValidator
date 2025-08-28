namespace VinValidator.Models;

public class AllData
{
    //Eam.Asset
    public string Asset { get; set; }
    public string SerialNumber { get; set; }

    //Ruan.Unit
    public string ChassisSerialNumber { get; set; }
    public string UnitCode { get; set; }

    //Lytx.VehicleSync
    public string UnitId { get; set; }
    public string VIN { get; set; }
}
