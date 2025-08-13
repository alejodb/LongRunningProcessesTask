namespace LongRunningProcesses.Dtos.ApiDtos;

public class CountTextOcurrencesRequestDto
{
  public required string Text { get; set; }
  public required string ConnectionId { get; set; }
}
