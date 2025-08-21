using System;
using System.Text;

namespace LongRunningProcesses.Application.UsesCases;

public class TestOcurrencesTasksPlanner
{
  public string GenerateTasksForProcessText(string text)
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
