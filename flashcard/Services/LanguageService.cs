using System.Diagnostics;
using System.Text.RegularExpressions;
using flashcard.Model;

namespace flashcard.Services
{
    public class LanguageService
    {
        private static readonly string appDirectory = FileSystem.Current.AppDataDirectory;
        private static readonly string WordsToLearnDirectory = System.IO.Path.Combine(appDirectory, "wordsToLearn.csv");

        public async Task<List<LanguageModel>> ReadWordsFromDataFile()
        {
            using var fileStream = await FileSystem.OpenAppPackageFileAsync("language_data.csv");
            using StreamReader? reader = new(fileStream);
            string line;
            List<LanguageModel> wordsList = new();
            while ((line = reader.ReadLine()!) != null)
            {
                //Define pattern
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                //Separating columns to array
                string[] parsedRow = CSVParser.Split(line);
                wordsList.Add(new LanguageModel { ForeignFormat = parsedRow[0], EnglishFormat = parsedRow[1] });
            }
            return wordsList!;
        }

        public async Task<List<LanguageModel>> ReadWordsToLearn()
        {
            using StreamReader? streamReader = new(WordsToLearnDirectory);
            string? line;
            List<LanguageModel> wordsToLearnList = new();
            while ((line = await streamReader.ReadLineAsync()!) != null)
            {
                //Define pattern
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                //Separating columns to array
                string[] parsedRow = CSVParser.Split(line);
                wordsToLearnList.Add(new LanguageModel { ForeignFormat = parsedRow[0], EnglishFormat = parsedRow[1] });
            }
            return wordsToLearnList;
        }

        public async Task<bool> WordInWordsToLearn(string foreignWord)
        {
            var wordsList = await ReadWordsToLearn();
            LanguageModel? word = wordsList.FirstOrDefault(word => word.ForeignFormat == foreignWord);
            if (word is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task AddToWordsToLearn(LanguageModel newWord)
        {
            var newLine = string.Format("{0},{1}", newWord.ForeignFormat, newWord.EnglishFormat);
            using StreamWriter streamWriter = new(WordsToLearnDirectory, true);

            if (!await WordInWordsToLearn(newWord.ForeignFormat!))
            {
                await streamWriter.WriteLineAsync(newLine);
                streamWriter.Flush();
            }
            else
            {
                Debug.WriteLine($"{newWord.ForeignFormat} already in words_to_learn_list");
            }
        }
    }
}
