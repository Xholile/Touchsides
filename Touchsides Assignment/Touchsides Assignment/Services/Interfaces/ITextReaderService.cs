namespace Touchsides_Assignment.Services.Interfaces
{
    public interface ITextReaderService
    {
        (string Word, int Count) GetMostFrequentWord(string content);
        (string Word, int Count) GetMostFrequent7CharacterWord(string content);
        (IEnumerable<string> Words, int Score) GetHighestScoringWords(string content);
    }
}
