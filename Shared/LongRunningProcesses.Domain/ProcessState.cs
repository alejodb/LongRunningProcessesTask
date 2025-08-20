using System.Diagnostics;

namespace LongRunningProcesses.Domain;

public class ProcessState
{
  public required string ProcessId { get; set; }
  public required string Status { get; set; }
  public int ProgressPosition { get; set; }
}
