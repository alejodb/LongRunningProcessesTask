using LongRunningProcesses.Dtos.Queues;

namespace LongRunningProcesses.Application.Interfaces;

public interface IMessagesConsumerService
{
  Task ConsumeCountTextOcurrencesMessage(CountTextOcurrencesMessageDto countTextOcurrencesMessageDto);
  Task ConsumeCancelProcessMessage(CancelProcessMessageDto cancelProcessMessageDto);
}