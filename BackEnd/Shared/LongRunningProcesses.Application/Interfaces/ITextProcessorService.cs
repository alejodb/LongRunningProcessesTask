using LongRunningProcesses.Dtos.Apis;

namespace LongRunningProcesses.Application.Interfaces;

public interface ITextProcessorService
{
  Task<CountTextOcurrencesResponseDto> CountTextOcurrences(CountTextOcurrencesRequestDto countTextOcurrencesRequestDto);
  Task CancelCountTextOcurrencesProcess(string processId);
}
