using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PassIn.Application.UseCases.Checkins.DoCheckin;
using PassIn.Communication.Responses;

namespace PassIn.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ILogger<CheckInController> _logger;
        private readonly DoAttendeeCheckInUseCase _doAttendeeCheckInUseCase;

        public CheckInController(ILogger<CheckInController> logger, DoAttendeeCheckInUseCase doAttendeeCheckInUseCase)
        {
            _logger = logger;
            _doAttendeeCheckInUseCase = doAttendeeCheckInUseCase;
        }

        [HttpPost]
        [Route("{attendeeId}")]
        [ProducesResponseType(typeof(ResponseRegisteredJson),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status409Conflict)]
        public IActionResult CheckIn([FromRoute] Guid attendeeId)
        {
            _logger.LogInformation("Received CheckIn request for Attendee ID: {AttendeeId}", attendeeId);
            var response = _doAttendeeCheckInUseCase.Execute(attendeeId);
            _logger.LogInformation("CheckIn request processed successfully for Attendee ID: {AttendeeId}", attendeeId);
            return Created(string.Empty, response);
        }
    }
}