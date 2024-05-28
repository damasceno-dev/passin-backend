using System.Net.Mail;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PassIn.Application.UseCases.Events.RegisterAttendee
{
    public class RegisterAttendeeOnEventUseCase
    {
        private readonly PassInDbContext _dbContext;
        private readonly ILogger<RegisterAttendeeOnEventUseCase> _logger;

        public RegisterAttendeeOnEventUseCase(ILogger<RegisterAttendeeOnEventUseCase> logger)
        {
            _dbContext = new PassInDbContext();
            _logger = logger;
        }

        public ResponseRegisteredJson Execute(RequestRegisterEventJson request, Guid eventId)
        {
            string requestJson = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Executing RegisterAttendeeOnEventUseCase for eventId: {eventId} with data: {requestJson}", eventId, requestJson);

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

            _logger.LogInformation("Attendee registered successfully with ID: {attendeeId}", newAttendee.Id);

            return new ResponseRegisteredJson
            {
                Id = newAttendee.Id
            };
        }

        private void Validate(Guid eventId, RequestRegisterEventJson request)
        {
            _logger.LogInformation("Validating request for eventId: {eventId} with data: {requestJson}", eventId, JsonConvert.SerializeObject(request));

            var eventEntity = _dbContext.Events.Find(eventId);
            if (eventEntity is null)
            {
                _logger.LogWarning("Validation failed: Event with ID {eventId} does not exist", eventId);
                throw new NotFoundException("An event with this id does not exist.");
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogWarning("Validation failed: Name is invalid for eventId: {eventId}", eventId);
                throw new ErrorOnValidationException("The name is invalid");
            }
            if (!EmailIsValid(request.Email))
            {
                _logger.LogWarning("Validation failed: Email is invalid for eventId: {eventId}", eventId);
                throw new ErrorOnValidationException("The email is invalid");
            }

            var attendeeAlreadyRegistered = _dbContext.Attendees.Any(a => a.Email.Equals(request.Email) && a.EventId == eventId);
            if (attendeeAlreadyRegistered)
            {
                _logger.LogWarning("Validation failed: Attendee already registered for eventId: {eventId}", eventId);
                throw new ConflictException("You can't register twice for the same event.");
            }

            var attendeesNumberForEvent = _dbContext.Attendees.Count(a => a.EventId == eventId);
            if (attendeesNumberForEvent == eventEntity.MaximumAttendees)
            {
                _logger.LogWarning("Validation failed: No room for eventId: {eventId}", eventId);
                throw new ConflictException("There is no room for this event");
            }

            _logger.LogInformation("Validation succeeded for eventId: {eventId}", eventId);
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
                _logger.LogWarning("Email validation failed: {email} is invalid", email);
                return false;
            }
        }
    }
}
