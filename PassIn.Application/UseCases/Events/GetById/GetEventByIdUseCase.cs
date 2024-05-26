using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Events.GetById
{
    public class GetEventByIdUseCase
    {
        private readonly ILogger<GetEventByIdUseCase> _logger;

        public GetEventByIdUseCase(ILogger<GetEventByIdUseCase> logger)
        {
            _logger = logger;
        }

        public ResponseEventJson Execute(Guid id)
        {
            _logger.LogInformation("Executing GetEventByIdUseCase with ID: {Id}", id);

            var dbContext = new PassInDbContext();
            var returnedEventId = dbContext.Events.Include(e => e.Attendees).FirstOrDefault(e => e.Id == id);
            if (returnedEventId is null)
            {
                _logger.LogWarning("Event with ID: {Id} not found.", id);
                throw new NotFoundException("An event with this id does not exist.");
            }

            _logger.LogInformation("Event with ID: {Id} found successfully.", id);

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
}