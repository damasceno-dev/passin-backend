using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.Register;

public class RegisterEventUseCase
{
    public ResponseRegisteredJson Execute(RequestEventJson request)
    {
        Validate(request);
        var dbContext = new PassInDbContext();
        var eventEntitty = new Event
        {
            Title = request.Title,
            Details = request.Details,
            MaximumAttendees = request.MaximumAttendees,
            Slug = request.Title.ToLower().Replace(" ", "-")
        };
        dbContext.Events.Add(eventEntitty);
        dbContext.SaveChanges();
        return new ResponseRegisteredJson
        {
            Id = eventEntitty.Id
        };
    }

    private void Validate(RequestEventJson request)
    {
        if (request.MaximumAttendees <= 0)
        {
            throw new ErrorOnValidationException("The maximum number of attendees is invalid");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ErrorOnValidationException("The title is invalid");
        }
        if (string.IsNullOrWhiteSpace(request.Details))
        {
            throw new ErrorOnValidationException("The details are invalid");
        }
    }
}