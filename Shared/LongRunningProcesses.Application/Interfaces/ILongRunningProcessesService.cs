using System;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Dtos.Queues;

namespace LongRunningProcesses.Application.Interfaces;

public interface ILongRunningProcessesService
{
  Task<CountTextOcurrencesResponseDto> CountTextOcurrences(CountTextOcurrencesRequestDto countTextOcurrencesRequestDto);
  Task CancelProcess(string processId);
  Task ProcessMessage(CountTextOcurrencesMessageDto message);
}
