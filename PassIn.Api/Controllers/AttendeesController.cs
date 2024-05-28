using Microsoft.AspNetCore.Mvc;
using PassIn.Application.UseCases.Attendees.GetAllByEventId;
using PassIn.Application.UseCases.Events.RegisterAttendee;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using Newtonsoft.Json;

namespace PassIn.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendeesController : ControllerBase
    {
        private readonly RegisterAttendeeOnEventUseCase _registerAttendeeOnEventUseCase;
        private readonly GetAllAttendeesByEventIdUseCase _getAllAttendeesByEventIdUseCase;
        private readonly ILogger<AttendeesController> _logger;

        public AttendeesController(
            RegisterAttendeeOnEventUseCase registerAttendeeOnEventUseCase,
            GetAllAttendeesByEventIdUseCase getAllAttendeesByEventIdUseCase,
            ILogger<AttendeesController> logger)
        {
            _registerAttendeeOnEventUseCase = registerAttendeeOnEventUseCase;
            _getAllAttendeesByEventIdUseCase = getAllAttendeesByEventIdUseCase;
            _logger = logger;
        }

        [HttpPost]
        [Route("{eventId}/register")]
        [ProducesResponseType(typeof(ResponseRegisteredJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
        public IActionResult Register([FromBody] RequestRegisterEventJson request, [FromRoute] Guid eventId)
        {
            _logger.LogInformation("Received Register request for eventId: {eventId} with data: {requestJson}", eventId, JsonConvert.SerializeObject(request));

            try
            {
                var response = _registerAttendeeOnEventUseCase.Execute(request, eventId);
                _logger.LogInformation("Register request processed successfully for eventId: {eventId}", eventId);
                return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Register request for eventId: {eventId}", eventId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorJson(ex.Message));
            }
        }

        [HttpGet]
        [Route("{eventId}")]
        [ProducesResponseType(typeof(ResponseAllAttendeesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public IActionResult GetAll([FromRoute] Guid eventId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string query = "")
        {
            _logger.LogInformation("Received GetAll request for eventId: {eventId}", eventId);

            try
            {
                var response = _getAllAttendeesByEventIdUseCase.Execute(eventId, pageNumber, pageSize, query);
                _logger.LogInformation("GetAll request processed successfully for eventId: {eventId}", eventId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GetAll request for eventId: {eventId}", eventId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorJson(ex.Message));
            }
        }
    }
}
