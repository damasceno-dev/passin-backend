using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Checkins.DoCheckin;

public class DoAttendeeCheckInUseCase
{
    private readonly PassInDbContext _dbContext;
    public DoAttendeeCheckInUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    public ResponseRegisteredJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);
        var checkin = new CheckIn
        {
            AttendeeId = attendeeId,
            CreatedAt = DateTime.UtcNow
            
        };
        _dbContext.CheckIns.Add(checkin);
        _dbContext.SaveChanges();
        return new ResponseRegisteredJson
        {
            Id = checkin.Id
        };
    }

    private void Validate(Guid attendeeId)
    {
        var existingAttendee = _dbContext.Attendees.Any(a => a.Id == attendeeId);
        if (!existingAttendee)
        {
            throw new NotFoundException("The attendee with this id was not found.");
        }

        var existingCheckIn = _dbContext.CheckIns.Any(c => c.AttendeeId == attendeeId);
        if (existingCheckIn)
        {
            throw new ConflictException("Attendee can't do check-in twice in the same event");
        }
    }
}