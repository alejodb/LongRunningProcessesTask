namespace LongRunningProcesses.Dtos.QueueDtos;

public class CountTextOcurrencesMessageDto
{
  public required string ProcessId { get; set; }
  public required string Text { get; set; }
  public required string ConnectionId { get; set; }
}