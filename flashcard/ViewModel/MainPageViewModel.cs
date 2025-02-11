using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using flashcard.Model;
using flashcard.Services;

namespace flashcard.ViewModel
{
    public partial class MainPageViewModel : BaseViewModel
    {
        readonly LanguageService _languageService;

        public ObservableCollection<LanguageModel> Words { get; } = new();

        public MainPageViewModel(LanguageService languageService)
        {
            _languageService = languageService;
            Title = "Language Flashcard";
            InitWordsList();
        }

        [RelayCommand]
        public async Task GetWordsAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            for (int i = 0; i < 5; i++)
            {
                Debug.WriteLine($"Word: {Words[i].ForeignFormat}, {Words[i].EnglishFormat}");
            }
            await Shell.Current.DisplayAlert("Info", "Retrived words from file", "Ok");
            IsBusy = false;
        }

        public async Task InitWordsList()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                var wordsToLearn = await GetKnwonWords();
                if (wordsToLearn != null)
                {
                    ParseWordsList(wordsToLearn);
                    IsBusy = false;
                    return;
                }
                var wordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(wordsList);
                IsBusy = false;
            }
            catch (FileNotFoundException exp)
            {
                var wordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(wordsList);
                IsBusy = false;
            }
        }

        public async Task<List<LanguageModel>> GetKnwonWords()
        {
            return await _languageService.ReadKnownWords();
        }

        public async Task<List<LanguageModel>> ReadWordsFromAppDataFile()
        {
            return await _languageService.ReadWordsFromDataFile();
        }

        public void ParseWordsList(List<LanguageModel> wordsList)
        {
            Words?.Clear();
            foreach (var word in wordsList)
                Words.Add(word);

        }

        [RelayCommand]
        public async Task AddToWordsToLearnAsync(LanguageModel wordToLearn)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                Debug.WriteLine($"Received {wordToLearn.ForeignFormat}, {wordToLearn.EnglishFormat}");
                await _languageService.AddToWordsToLearn(wordToLearn);
                Words.Remove(wordToLearn);
                Debug.WriteLine($"Words collection count: {Words.Count}");
                IsBusy = false;
            }
            catch (System.Exception exp)
            {
                Debug.WriteLine($"Exception: {exp.Message}");
                IsBusy = false;
            }
        }
    }
}


