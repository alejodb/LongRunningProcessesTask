using Microsoft.AspNetCore.Mvc;
using LongRunningProcesses.Dtos.ApiDtos;
using LongRunningProcesses.Dtos.QueueDtos;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;

namespace LongRunningProcesses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextProcessorController : ControllerBase
    {
        private readonly ILogger<TextProcessorController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IDistributedCache _redisCache;
        public TextProcessorController(ILogger<TextProcessorController> logger, IPublishEndpoint publishEndpoint, IDistributedCache redisCache)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _redisCache = redisCache;
        }

        [HttpPost]
        public async Task<IActionResult> CountTextOcurrences([FromBody] CountTextOcurrencesRequestDto countTextOcurrencesRequestDto)
        {
            var processId = Guid.NewGuid().ToString("N");

            _logger.LogInformation($"Received request to count text occurrences, ProcessId: {processId}, ConnectionId: {countTextOcurrencesRequestDto.ConnectionId}");

            await _redisCache.SetStringAsync($"process:status:{processId}", "pending");

            await _publishEndpoint.Publish(new CountTextOcurrencesMessageDto
            {
                ProcessId = processId,
                Text = countTextOcurrencesRequestDto.Text,
                ConnectionId = countTextOcurrencesRequestDto.ConnectionId
            });

            return Accepted(new CountTextOcurrencesResponseDto { ProcessId = processId });
        }

        [HttpDelete("{processId}")]
        public async Task<IActionResult> CancelProcess([FromRoute] string processId)
        {
            _logger.LogInformation($"Received request to cancel process, ProcessId: {processId}");

            await _redisCache.SetStringAsync($"process:status:{processId}", "canceled");

            return Ok();
        }
    }
}
