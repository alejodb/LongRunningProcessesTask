namespace LongRunningProcesses.Application.Interfaces;

public interface IMessagePublisher
{
  Task Publish<T>(T message) where T : notnull;
}
