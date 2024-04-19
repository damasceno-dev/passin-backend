using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Attendees.GetAllByEventId;

public class GetAllAttendeesByEventIdUseCase
{
    private readonly PassInDbContext _dbContext;

    public GetAllAttendeesByEventIdUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    public ResponseAllAttendeesJson Execute(Guid eventId, int pageNumber, int pageSize, string query)
    {
        //dont use foreign key method:
        //var attendeesNoFK =  _dbContext.Attendees.Where(a => a.EventId == eventId).ToList();
        
        //using foreign key:
        //var eventEntity = _dbContext.Events.Find(eventId);
        
        //if the above does not work:
         var eventEntity = _dbContext.Events.Include(e=>e.Attendees).ThenInclude(attendee => attendee.CheckIn)
             .FirstOrDefault(e=> e
             .Id == 
             eventId);
        
        if (eventEntity is null)
        {
            throw new NotFoundException("An event with this id does not exists.");
        }
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ErrorOnValidationException("Page number and Page size should be both greater than 0.");
        }
        
        var filteredAttendees = string.IsNullOrWhiteSpace(query) 
            ? eventEntity.Attendees 
            : eventEntity.Attendees.Where(a => a.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                               a.Email.Contains(query,StringComparison.OrdinalIgnoreCase));

       
        var totalAttendees = filteredAttendees.Count();
        var totalPages = (int)Math.Ceiling((double)totalAttendees / pageSize);
        
        return new ResponseAllAttendeesJson
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
    }
}