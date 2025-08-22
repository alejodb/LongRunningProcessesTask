using LongRunningProcesses.Application.Services;
using LongRunningProcesses.Application.UsesCases;
using LongRunningProcesses.Dtos.Queues;
using Microsoft.Extensions.Logging;
using Moq;

public class MessagesConsumerService_Tests
{
  [Fact]
  public async Task IsConsumeCountTextOcurrencesMessage_Success()
  {
    var longRunningProcessesOrchestatorLoggerMock = new Mock<ILogger<LongRunningProcessesOrchestator>>();
    var cancellationMonitorLoggerMock = new Mock<ILogger<CancellationMonitor>>();
    var processStateRepositoryMock = new ProcessStateRepositoryMock();
    var asyncCommunicationsProviderMock = new AsyncCommunicationsProviderMock();

    var testOcurrencesTasksPlanner = new TestOcurrencesTasksPlanner();
    var cancellationMonitor = new CancellationMonitor(cancellationMonitorLoggerMock.Object, processStateRepositoryMock);
    var longRunningProcessesOrchestator = new LongRunningProcessesOrchestator(
      longRunningProcessesOrchestatorLoggerMock.Object,
      asyncCommunicationsProviderMock,
      testOcurrencesTasksPlanner,
      cancellationMonitor,
      processStateRepositoryMock,
      0);
    var messageConsumerService = new MessagesConsumerService(processStateRepositoryMock, longRunningProcessesOrchestator);
    await messageConsumerService.ConsumeCountTextOcurrencesMessage(new CountTextOcurrencesMessageDto("ProcessId1", "Hello World!", "Connection1"));

    Assert.Equal(" 1!1H1W1d1e1l3o2r1/SGVsbG8gV29ybGQh", string.Join("", asyncCommunicationsProviderMock.ResponseMessages));
    Assert.Single(asyncCommunicationsProviderMock.StatusMessages);
    Assert.Equal("Long-running process COMPLETED successfully.", string.Join("", asyncCommunicationsProviderMock.StatusMessages));
  }

  [Fact]
  public async Task IsConsumeCancelProcessMessage_Success()
  {
    var longRunningProcessesOrchestatorLoggerMock = new Mock<ILogger<LongRunningProcessesOrchestator>>();
    var cancellationMonitorLoggerMock = new Mock<ILogger<CancellationMonitor>>();
    var processStateRepositoryMock = new ProcessStateRepositoryMock();
    var asyncCommunicationsProviderMock = new AsyncCommunicationsProviderMock();

    var testOcurrencesTasksPlanner = new TestOcurrencesTasksPlanner();
    var cancellationMonitor = new CancellationMonitor(cancellationMonitorLoggerMock.Object, processStateRepositoryMock);
    var longRunningProcessesOrchestator = new LongRunningProcessesOrchestator(
      longRunningProcessesOrchestatorLoggerMock.Object,
      asyncCommunicationsProviderMock,
      testOcurrencesTasksPlanner,
      cancellationMonitor,
      processStateRepositoryMock,
      100);
    var messageConsumerService = new MessagesConsumerService(processStateRepositoryMock, longRunningProcessesOrchestator);

    var consumeCountTextOcurrencesTask = messageConsumerService.ConsumeCountTextOcurrencesMessage(new CountTextOcurrencesMessageDto("ProcessId1", "Hello World!", "Connection1"));
    await Task.Delay(50);
    await messageConsumerService.ConsumeCancelProcessMessage(new CancelProcessMessageDto("ProcessId1"));
    await consumeCountTextOcurrencesTask;

    Assert.Single(asyncCommunicationsProviderMock.StatusMessages);
    Assert.Equal("Long-running process CANCELED.", string.Join("", asyncCommunicationsProviderMock.StatusMessages));
  }
}