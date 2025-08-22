using Microsoft.AspNetCore.Mvc;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Application.Interfaces;

namespace LongRunningProcesses.ApiService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TextProcessorController (ITextProcessorService longRunningProcessesService) : ControllerBase
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
