using LongRunningProcesses.Application.Interfaces;

public class MessagesPublisherMock : IMessagePublisher
{
  private readonly List<object> publishedMessages = new List<object>();
  public IReadOnlyList<object> PublishedMessages => publishedMessages;
  public Task Publish<T>(T message) where T : notnull
  {
    publishedMessages.Add(message);
    return Task.CompletedTask;
  }
}