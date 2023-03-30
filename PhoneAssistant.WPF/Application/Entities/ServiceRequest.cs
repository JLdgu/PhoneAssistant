namespace PhoneAssistant.WPF.Application.Entities;

public class ServiceRequest
{
    public int Id { get; set; }
    public virtual Phone? PhoneEntity { get; set; }
    public int? PhoneEntityId { get; set; }
    public virtual Sim? SimEntity { get; set; }
    public int? SimEntityId { get; set; }
    public int ServiceRequestNumber { get; set; }
    public string NewUser { get; set; }
    public string? DespatchDetails { get; set; }
}