using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;
using System;

namespace PassIn.Application.UseCases.Attendees.GetAllByEventId
{
    public class GetAllAttendeesByEventIdUseCase
    {
        private readonly PassInDbContext _dbContext;
        private readonly ILogger<GetAllAttendeesByEventIdUseCase> _logger;

        public GetAllAttendeesByEventIdUseCase(ILogger<GetAllAttendeesByEventIdUseCase> logger)
        {
            _dbContext = new PassInDbContext();
            _logger = logger;
        }

        public ResponseAllAttendeesJson Execute(Guid eventId, int pageNumber, int pageSize, string query)
        {
            _logger.LogInformation("Executing GetAllAttendeesByEventIdUseCase with eventId: {EventId}, pageNumber: {PageNumber}, pageSize: {PageSize}, query: {Query}", eventId, pageNumber, pageSize, query);

            var eventEntity = _dbContext.Events.Include(e => e.Attendees).ThenInclude(attendee => attendee.CheckIn)
                .FirstOrDefault(e => e.Id == eventId);

            if (eventEntity is null)
            {
                _logger.LogWarning("Event with id {EventId} not found.", eventId);
                throw new NotFoundException("An event with this id does not exists.");
            }

            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid page number or page size. Page number: {PageNumber}, Page size: {PageSize}", pageNumber, pageSize);
                throw new ErrorOnValidationException("Page number and Page size should be both greater than 0.");
            }

            var filteredAttendees = string.IsNullOrWhiteSpace(query)
                ? eventEntity.Attendees
                : eventEntity.Attendees.Where(a => a.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                   a.Email.Contains(query, StringComparison.OrdinalIgnoreCase));

            var totalAttendees = filteredAttendees.Count();
            var totalPages = (int)Math.Ceiling((double)totalAttendees / pageSize);

            var response = new ResponseAllAttendeesJson
            {
                Total = totalAttendees,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Attendees = filteredAttendees
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(attendee => new ResponseAttendeeJson()
                    {
                        Id = attendee.Id,
                        Name = attendee.Name,
                        Email = attendee.Email,
                        CreatedAt = attendee.CreatedAt,
                        CheckedInAt = attendee.CheckIn?.CreatedAt
                    }).ToList(),
            };

            _logger.LogInformation("GetAllAttendeesByEventIdUseCase executed successfully for eventId: {EventId}", eventId);
            return response;
        }
    }
}
