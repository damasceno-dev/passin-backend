using Microsoft.Extensions.Logging;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Checkins.DoCheckin;

public class DoAttendeeCheckInUseCase
{
    private readonly PassInDbContext _dbContext;
    private readonly ILogger<DoAttendeeCheckInUseCase> _logger;

    public DoAttendeeCheckInUseCase(ILogger<DoAttendeeCheckInUseCase> logger)
    {
        _dbContext = new PassInDbContext();
        _logger = logger;
    }

    public ResponseRegisteredJson Execute(Guid attendeeId)
    {
        _logger.LogInformation("Executing CheckIn for Attendee ID: {AttendeeId}", attendeeId);
        
        Validate(attendeeId);
        
        var checkin = new CheckIn
        {
            AttendeeId = attendeeId,
            CreatedAt = DateTime.UtcNow
        };
        
        _dbContext.CheckIns.Add(checkin);
        _dbContext.SaveChanges();

        _logger.LogInformation("CheckIn successful for Attendee ID: {AttendeeId}", attendeeId);

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
            _logger.LogWarning("Validation failed: Attendee ID {AttendeeId} not found", attendeeId);
            throw new NotFoundException("The attendee with this id was not found.");
        }

        var existingCheckIn = _dbContext.CheckIns.Any(c => c.AttendeeId == attendeeId);
        if (existingCheckIn)
        {
            _logger.LogWarning("Validation failed: Attendee ID {AttendeeId} already checked in", attendeeId);
            throw new ConflictException("Attendee can't do check-in twice in the same event");
        }
    }
}