using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Application;

public class ServiceRequestEntity
{
    public int Id { get; set; }
    public virtual PhoneEntity? PhoneEntity { get; set; }
    public int? PhoneEntityId { get; set; }
    public virtual SimEntity? SimEntity { get; set; }
    public int? SimEntityId { get; set; }
    public int? ServiceRequest { get; set; }
    public string? NewUser { get; set; }
    public string? DespatchDetails { get; set; }
    public string? Analyst { get; set; }
}