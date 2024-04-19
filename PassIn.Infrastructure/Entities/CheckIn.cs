namespace PassIn.Infrastructure.Entities;

public class CheckIn
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public Guid AttendeeId { get; set; }
    public Attendee Attendee { get; set; } = default!; //can't be null when its going to check-in
}