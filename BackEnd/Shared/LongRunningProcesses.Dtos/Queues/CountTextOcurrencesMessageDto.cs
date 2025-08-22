namespace LongRunningProcesses.Dtos.Queues;

public record CountTextOcurrencesMessageDto(string ProcessId, string Text, string ConnectionId);