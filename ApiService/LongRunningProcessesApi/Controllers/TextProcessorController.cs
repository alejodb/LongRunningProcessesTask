using Microsoft.AspNetCore.Mvc;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Dtos.Queues;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;

namespace LongRunningProcessesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextProcessorController (ILogger<TextProcessorController> logger, IPublishEndpoint publishEndpoint, IDistributedCache redisCache) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CountTextOcurrences([FromBody] CountTextOcurrencesRequestDto countTextOcurrencesRequestDto)
        {
            var processId = Guid.NewGuid().ToString("N");

            logger.LogInformation($"Received request to count text occurrences, ProcessId: {processId}, ConnectionId: {countTextOcurrencesRequestDto.ConnectionId}");
            await redisCache.SetStringAsync($"process:status:{processId}", "pending");

            await publishEndpoint.Publish(new CountTextOcurrencesMessageDto(processId, countTextOcurrencesRequestDto.Text, countTextOcurrencesRequestDto.ConnectionId));

            return Accepted(new CountTextOcurrencesResponseDto(processId));
        }

        [HttpDelete("{processId}")]
        public async Task<IActionResult> CancelProcess([FromRoute] string processId)
        {
            logger.LogInformation($"Received request to cancel process, ProcessId: {processId}");

            await redisCache.SetStringAsync($"process:status:{processId}", "canceled");

            return Ok();
        }
    }
}
