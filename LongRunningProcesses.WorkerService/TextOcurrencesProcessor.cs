using System;
using System.Text;
using LongRunningProcesses.Dtos.QueueDtos;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Distributed;

namespace LongRunningProcesses.WorkerService;

public class TextOcurrencesProcessor
{
  private readonly ILogger<TextOcurrencesProcessor> _logger;
  private readonly IDistributedCache _redisCache;
  private readonly HubConnection _hubConnection;

  public TextOcurrencesProcessor(ILogger<TextOcurrencesProcessor> logger, IDistributedCache redisCache, IConfiguration configuration)
  {
    _logger = logger;
    _redisCache = redisCache;

    var apiUrl = configuration["services:apiservice:http:0"];
    _hubConnection = new HubConnectionBuilder()
        .WithUrl($"{apiUrl}/chatHub")
        .Build();
  }

  public async Task ProcessMessage(CountTextOcurrencesMessageDto message)
  {
    await _redisCache.SetStringAsync($"process:status:{message.ProcessId}", "processing");
    await _hubConnection.StartAsync();

    try
    {
      await GenerateAndSendSignedTextOcurrences(message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error processing message for process {message.ProcessId}");
      await _redisCache.SetStringAsync($"process:status:{message.ProcessId}", "failed, the process will be retried");
      throw;
    }
    finally
    {
      await _hubConnection.DisposeAsync();
    }
  }

  private async Task GenerateAndSendSignedTextOcurrences(CountTextOcurrencesMessageDto message)
  {
    var signedTextOcurrences = GenerateSignedTextOcurrencesString(message.Text);
    _logger.LogInformation($"Generated signed text occurrences for process {message.ProcessId}: {signedTextOcurrences}");
    var randomizer = new Random();
    var currentCharacter = await _redisCache.GetStringAsync($"process:currentCharacter:{message.ProcessId}");

    if (!string.IsNullOrEmpty(currentCharacter))
    {
      _logger.LogInformation($"Resuming process {message.ProcessId} from character position {currentCharacter}");
    }
    else
    {
      _logger.LogInformation($"Starting new process {message.ProcessId} for text: {message.Text}");
    }

    var position = !string.IsNullOrEmpty(currentCharacter) ? int.Parse(currentCharacter) : 0;

    while (position < signedTextOcurrences.Length)
    {
      var character = signedTextOcurrences[position];
      var delay = randomizer.Next(5000);
      await Task.Delay(delay);

      // Uncomment this line for testing exceptions and retries
      /*if (position == 5)
      {
        throw new InvalidOperationException($"Simulated error at position {position} for process {message.ProcessId}");
      }*/

      var status = await _redisCache.GetStringAsync($"process:status:{message.ProcessId}");
      if (status != "canceled")
      {
        position++;
        await _hubConnection.InvokeAsync("SendResponseMessage", message.ConnectionId, character.ToString());
        await _redisCache.SetStringAsync($"process:currentCharacter:{message.ProcessId}", $"{position}");
        _logger.LogInformation($"Processed character '{character}' at position {position} for process {message.ProcessId}");
      }
      else
      {
        _logger.LogInformation($"Process {message.ProcessId} was canceled.");
        await _hubConnection.InvokeAsync("SendEndMessage", message.ConnectionId, $"Long-running process CANCELED successfully.");
        return;
      }
    }
    await _hubConnection.InvokeAsync("SendEndMessage", message.ConnectionId, $"Long-running process COMPLETED successfully.");

    await _redisCache.RemoveAsync($"process:currentCharacter:{message.ProcessId}");
    await _redisCache.RemoveAsync($"process:status:{message.ProcessId}");
  }

  private string GenerateSignedTextOcurrencesString(string text)
  {
    var base64EncodedText = GenerateBase64EncodedText(text);
    var textOcurrences = GenerateTextOcurrencesString(text);
    var signedTextOcurrences = $"{textOcurrences}/{base64EncodedText}";

    return signedTextOcurrences;
  }

  private string GenerateTextOcurrencesString(string text)
  {
    return FormatCharacterCount(CountTextCharactersOcurrences(text));
  }

  private IDictionary<char, int> CountTextCharactersOcurrences(string text)
  {
    var charactersCount = new Dictionary<char, int>();

    foreach (var character in text)
    {
      if (charactersCount.ContainsKey(character))
      {
        charactersCount[character]++;
      }
      else
      {
        charactersCount[character] = 1;
      }
    }

    return charactersCount.OrderBy(keyValuePair => keyValuePair.Key)
                          .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
  }

  private string FormatCharacterCount(IDictionary<char, int> characterCounts)
  {
    return string.Join("", characterCounts.Select(keyValuePair => $"{keyValuePair.Key}{keyValuePair.Value}"));
  }

  private string GenerateBase64EncodedText(string text)
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
  }
}
