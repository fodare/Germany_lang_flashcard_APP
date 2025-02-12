using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using flashcard.Model;
using flashcard.Services;

namespace flashcard.ViewModel
{
    public partial class MainPageViewModel : BaseViewModel
    {
        readonly LanguageService _languageService;

        public ObservableCollection<LanguageModel> Words { get; } = new();

        public Random random = new();

        public MainPageViewModel(LanguageService languageService)
        {
            _languageService = languageService;
            Title = "Language Flashcard";
            InitWordsList();
            GetWordsAsync();
        }

        [ObservableProperty]
        public bool isVisible;

        [ObservableProperty]
        public string? foreignFormat;

        [ObservableProperty]
        public string? englishFormat;

        [RelayCommand]
        public async Task GetWordsAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;
            await GetRandomWord();
            await Task.Delay(3000);
            IsVisible = true;
            IsBusy = false;
        }

        public async Task GetRandomWord()
        {
            if (Words.Count == 0)
            {
                var newWordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(newWordsList);
            }

            var randomWord = Words.ElementAt(random.Next(0, Words.Count));
            ForeignFormat = randomWord.ForeignFormat;
            EnglishFormat = randomWord.EnglishFormat;
        }

        public async Task NextCard()
        {
            await GetRandomWord();
            IsVisible = false;
            await Task.Delay(3000);
            IsVisible = true;
        }

        public async Task InitWordsList()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                var wordsToLearn = await GetWordsToLearn();
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
            catch (FileNotFoundException)
            {
                var wordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(wordsList);
                IsBusy = false;
            }
        }

        public async Task<List<LanguageModel>> GetWordsToLearn()
        {
            return await _languageService.ReadWordsToLearn();
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
        public async Task AddToWordsToLearnAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                Debug.WriteLine($"{ForeignFormat},{EnglishFormat}");
                //await _languageService.AddToWordsToLearn(wordToLearn);
                //Words.Remove(wordToLearn);
                await NextCard();
                //Debug.WriteLine($"Words collection count: {Words.Count}");
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


