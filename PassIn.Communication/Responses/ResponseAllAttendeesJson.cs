namespace PassIn.Communication.Responses;
public class ResponseAllAttendeesJson
{
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<ResponseAttendeeJson> Attendees { get; set; } = [];
    
}
