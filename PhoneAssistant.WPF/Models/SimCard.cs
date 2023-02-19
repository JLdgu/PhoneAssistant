namespace PhoneAssistant.Models;

public sealed class SimCard
{
    public int Id { get; init; }

    public string PhoneNumber { get; init; }

    public string SimNumber { get; init; }

    public string Status { get; init; }

    public string? Note { get; init; }

    //public SimCard(int id, string phoneNumber, string simNumber, string? formerUser, string status)
    //{   
    //    Id = id;
    //    PhoneNumber = phoneNumber;
    //    SimNumber = simNumber;
    //    FormerUser = formerUser;
    //    Status = status;
    //}

    //public static SimCard ToSimCard(SimCardDTO simCardDTO)
    //{
    //    return new SimCard(
    //        simCardDTO.Id,
    //        simCardDTO.PhoneNumber,
    //        simCardDTO.SimNumber,
    //        simCardDTO.FormerUser, 
    //        simCardDTO.State.Status); 
    //}

    //public static implicit operator SimCardDTO(SimCard simCard)
    //{        
    //    return new SimCardDTO
    //    {
    //        Id = simCard.Id,
    //        PhoneNumber = simCard.PhoneNumber,
    //        SimNumber = simCard.SimNumber,
    //        FormerUser = simCard.FormerUser,
    //        State = new() { Status = simCard.Status }
    //    };
    //}
}
