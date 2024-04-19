using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Events.GetById;

public class GetEventByIdUseCase
{
    public ResponseEventJson Execute(Guid id)
    {
        var dbContext = new PassInDbContext();
        var returnedEventId = dbContext.Events.Include(e => e.Attendees).FirstOrDefault(e=> e.Id == id);
        if (returnedEventId is null)
        {
            throw new NotFoundException("An event with this id does not exists.");
        }

        return new ResponseEventJson
        {
            Id = returnedEventId.Id,
            Title = returnedEventId.Title,
            Details = returnedEventId.Details,
            MaximumAttendees = returnedEventId.MaximumAttendees,
            AttendeesAmount = returnedEventId.Attendees.Count()
        };
    }
}