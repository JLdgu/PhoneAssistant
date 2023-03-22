namespace PhoneAssistant.WPF.Application.Entities;

public class StateEntity
{
    public string Status { get; set; }
    
    public StateEntity(string status)
    {
        Status = status;
    }

    // Navigation properties
    //public ICollection<PhoneDTO> Phones { get; set; } = new ObservableCollection<PhoneDTO>();
}
