using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PassIn.Application.UseCases.Events.GetById;
using PassIn.Application.UseCases.Events.Register;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using ResponseErrorJson = PassIn.Communication.Responses.ResponseErrorJson;

namespace PassIn.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly RegisterEventUseCase _registerEventUseCase;
        private readonly GetEventByIdUseCase _getEventByIdUseCase;

        public EventsController(ILogger<EventsController> logger, RegisterEventUseCase registerEventUseCase, GetEventByIdUseCase getEventByIdUseCase)
        {
            _logger = logger;
            _registerEventUseCase = registerEventUseCase;
            _getEventByIdUseCase = getEventByIdUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RequestEventJson request)
        {
            _logger.LogInformation("Received Register request with data: {@Request}", request);

            try
            {
                var response = _registerEventUseCase.Execute(request);
                _logger.LogInformation("Register request processed successfully.");
                return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Register request.");
                return BadRequest(new ResponseErrorJson(message:ex.Message));
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseEventJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] Guid id)
        {
            _logger.LogInformation("Received GetById request for ID: {Id}", id);

            try
            {
                var response = _getEventByIdUseCase.Execute(id);
                _logger.LogInformation("GetById request processed successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GetById request for ID: {Id}", id);
                return NotFound(new ResponseErrorJson(message:ex.Message));
            }
        }
    }
}
