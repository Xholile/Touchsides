using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.IO;
using System.Text;
using Touchsides_Assignment.Services.Interfaces;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Touchsides_Assignment.Services
{
    public class TextReaderService : ITextReaderService
    {

        private readonly Dictionary<char, int> _letterScores = new Dictionary<char, int>
        {
            {'A', 1}, {'B', 3}, {'C', 3}, {'D', 2}, {'E', 1}, {'F', 4}, {'G', 2}, {'H', 4},
            {'I', 1}, {'J', 8}, {'K', 5}, {'L', 1}, {'M', 3}, {'N', 1}, {'O', 1}, {'P', 3},
            {'Q', 10}, {'R', 1}, {'S', 1}, {'T', 1}, {'U', 1}, {'V', 4}, {'W', 4}, {'X', 8},
            {'Y', 4}, {'Z', 10}
        };

        public (string Word, int Count) GetMostFrequentWord(string content)
        {
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var words = content.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var wordWithoutAlphaChars = Regex.Replace(word.ToLower(), @"[^a-z]+", "");

                if (!string.IsNullOrEmpty(wordWithoutAlphaChars))
                {
                    if (wordCounts.ContainsKey(wordWithoutAlphaChars))
                    {
                        wordCounts[wordWithoutAlphaChars]++;
                    }
                    else
                    {
                        wordCounts[wordWithoutAlphaChars] = 1;
                    }
                }
            }

            var mostFrequentWord = wordCounts.OrderByDescending(w => w.Value).FirstOrDefault();

            return (mostFrequentWord.Key, mostFrequentWord.Value);
        }

        public (string Word, int Count) GetMostFrequent7CharacterWord(string content)
        {
            var words = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            var sevenCharacterWords = words
                .Select(w => Regex.Replace(w.ToLower(), @"[^a-z]+", ""))
                .Where(cleanedWord => cleanedWord.Length == 7);

            var frequency = sevenCharacterWords
                .GroupBy(w => w)
                .Select(group => new { Word = group.Key, Count = group.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            return frequency == null ? (string.Empty, 0) : (frequency.Word, frequency.Count);
        }

        public (IEnumerable<string> Words, int Score) GetHighestScoringWords(string content)
        {
            var cleanedContent = Regex.Replace(content, @"[^a-zA-Z\s]", " ");
            var words = cleanedContent.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var wordScores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int highestScore = 0;
            foreach (var word in words)
            {
                int score = word.ToUpper().Sum(c => _letterScores.ContainsKey(c) ? _letterScores[c] : 0);
                if (wordScores.ContainsKey(word))
                {
                    wordScores[word] = Math.Max(wordScores[word], score);
                }
                else
                {
                    wordScores[word] = score;
                }
                highestScore = Math.Max(highestScore, score);
            }
            var highestScoringWords = wordScores
                .Where(w => w.Value == highestScore)
                .Select(w => w.Key);

            return (highestScoringWords, highestScore);
        }
    }
}