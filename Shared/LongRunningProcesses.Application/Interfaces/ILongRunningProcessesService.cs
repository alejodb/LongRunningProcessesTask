using System;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Dtos.Queues;

namespace LongRunningProcesses.Application.Interfaces;

public interface ILongRunningProcessesService
{
  Task<CountTextOcurrencesResponseDto> CountTextOcurrences(CountTextOcurrencesRequestDto countTextOcurrencesRequestDto);
  Task CancelCountTextOcurrencesProcess(string processId);
  Task ProcessCountTextOcurrencesMessage(CountTextOcurrencesMessageDto message);
}
