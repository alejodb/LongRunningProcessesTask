using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Application.UsesCases;
using LongRunningProcesses.Dtos.Queues;

namespace LongRunningProcesses.Application.Services;

public class MessagesConsumerService(
  IProcessStateRepository processStateRepository,
  LongRunningProcessesOrchestator longRunningProcessesOrchestator) 
  : IMessagesConsumerService
{
  public async Task ConsumeCountTextOcurrencesMessage(CountTextOcurrencesMessageDto countTextOcurrencesMessageDto)
  {
    await longRunningProcessesOrchestator.StartTextOcurrencesProcess(countTextOcurrencesMessageDto);
  }

  public async Task ConsumeCancelProcessMessage(CancelProcessMessageDto cancelProcessMessageDto)
  {
    await processStateRepository.SaveCanceledAsync(cancelProcessMessageDto.ProcessId);
  }
}