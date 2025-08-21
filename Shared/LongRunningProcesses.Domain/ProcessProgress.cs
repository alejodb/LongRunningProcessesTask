namespace LongRunningProcesses.Domain;

public class ProcessProgress
{
  public required string ProcessId { get; set; }
  public int Position { get; set; }
  public required string SignedTextOccurrences { get; set; }
  public required string ConnectionId { get; set; }
}
