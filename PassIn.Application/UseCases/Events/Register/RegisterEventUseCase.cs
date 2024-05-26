using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.Register
{
    public class RegisterEventUseCase
    {
        private readonly ILogger<RegisterEventUseCase> _logger;

        public RegisterEventUseCase(ILogger<RegisterEventUseCase> logger)
        {
            _logger = logger;
        }

        public ResponseRegisteredJson Execute(RequestEventJson request)
        {
            string requestJson = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Executing RegisterEventUseCase with request: {RequestJson}", requestJson);

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

            _logger.LogInformation("Event registered successfully with ID: {EventId}", eventEntitty.Id);
            return new ResponseRegisteredJson
            {
                Id = eventEntitty.Id
            };
        }

        private void Validate(RequestEventJson request)
        {
            if (request.MaximumAttendees <= 0)
            {
                _logger.LogWarning("Validation failed: MaximumAttendees is less than or equal to 0");
                throw new ErrorOnValidationException("The maximum number of attendees is invalid");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("Validation failed: Title is null or whitespace");
                throw new ErrorOnValidationException("The title is invalid");
            }

            if (string.IsNullOrWhiteSpace(request.Details))
            {
                _logger.LogWarning("Validation failed: Details are null or whitespace");
                throw new ErrorOnValidationException("The details are invalid");
            }
        }
    }
}
