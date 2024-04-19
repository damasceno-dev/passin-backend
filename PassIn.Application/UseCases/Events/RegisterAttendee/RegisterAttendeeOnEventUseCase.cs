using System.Net.Mail;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;

public class RegisterAttendeeOnEventUseCase
{
    private readonly PassInDbContext _dbContext;
    public RegisterAttendeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    public ResponseRegisteredJson Execute(RequestRegisterEventJson request,  Guid eventId)
    {
        Validate(eventId, request);
        var newAttendee = new Infrastructure.Entities.Attendee
        {
            Email = request.Email,
            Name = request.Name,
            EventId = eventId,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Attendees.Add(newAttendee);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = newAttendee.Id
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _dbContext.Events.Find(eventId);
        if (eventEntity is null)
        {
            throw new NotFoundException("An event with this id does not exists.");
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("The name is invalid");
        }
        if (!EmailIsValid(request.Email))
        {
            throw new ErrorOnValidationException("The email is invalid");
        }

        var attendeeAlreadyRegistered = _dbContext.Attendees.Any(a => a.Email.Equals(request.Email) && a.EventId == 
            eventId);

        if (attendeeAlreadyRegistered)
        {
            throw new ConflictException("You can't register twice on the same event.");
        }

        var attendeesNumberForEvent = _dbContext.Attendees.Count(a => a.EventId == eventId);
        if (attendeesNumberForEvent == eventEntity.MaximumAttendees)
        {
            throw new ConflictException("There is no room for this event");
        }
    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}