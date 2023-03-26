namespace PhoneAssistant.WPF.Application.Entities;

public class State
{
    public string Status { get; set; }
    
    public State(string status)
    {
        Status = status;
    }

    // Navigation properties
    //public ICollection<PhoneDTO> Phones { get; set; } = new ObservableCollection<PhoneDTO>();
}
