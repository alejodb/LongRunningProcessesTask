using Microsoft.AspNetCore.Mvc;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Application.Interfaces;

namespace LongRunningProcessesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextProcessorController (ILongRunningProcessesService longRunningProcessesService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CountTextOcurrences([FromBody] CountTextOcurrencesRequestDto countTextOcurrencesRequestDto)
        {
            return Accepted(await longRunningProcessesService.CountTextOcurrences(countTextOcurrencesRequestDto));
        }

        [HttpDelete("{processId}")]
        public async Task<IActionResult> CancelProcess([FromRoute] string processId)
        {
            await longRunningProcessesService.CancelCountTextOcurrencesProcess(processId);
            return Ok();
        }
    }
}
