using System.Text.RegularExpressions;
using flashcard.Model;

namespace flashcard.Services
{
    public class LanguageService
    {
        private static string appDirectory = FileSystem.Current.AppDataDirectory;
        private static string WordsToLearnDirectory = System.IO.Path.Combine(appDirectory, "wordsToLearn.csv");

        private readonly List<LanguageModel> Words = new();

        public async Task<List<LanguageModel>> ReadWordsFromDataFile()
        {
            using var fileStream = await FileSystem.OpenAppPackageFileAsync("language_data.csv");
            using StreamReader? reader = new(fileStream);
            string line;
            while ((line = reader.ReadLine()!) != null)
            {
                //Define pattern
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                //Separating columns to array
                string[] parsedRow = CSVParser.Split(line);
                Words?.Add(new LanguageModel { ForeignFormat = parsedRow[0], EnglishFormat = parsedRow[1] });
            }
            return Words!;
        }

        public async Task AddToWordsToLearn(LanguageModel newWord)
        {
            using StreamWriter streamWriter = new(WordsToLearnDirectory);
            var newLine = string.Format("{0},{1}", newWord.ForeignFormat, newWord.EnglishFormat);
            await streamWriter.WriteLineAsync(newLine);
            streamWriter.Flush();
        }

        public async Task<List<LanguageModel>> ReadKnownWords()
        {
            using StreamReader? streamReader = new(WordsToLearnDirectory);
            string? line;
            while ((line = await streamReader.ReadLineAsync()!) != null)
            {
                //Define pattern
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                //Separating columns to array
                string[] parsedRow = CSVParser.Split(line);
                Words?.Add(new LanguageModel { ForeignFormat = parsedRow[0], EnglishFormat = parsedRow[1] });
            }
            return Words;
        }
    }
}
