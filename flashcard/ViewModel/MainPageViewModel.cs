using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using flashcard.Model;
using flashcard.Services;

namespace flashcard.ViewModel
{
    public partial class MainPageViewModel : BaseViewModel
    {
        readonly LanguageService _languageService;

        public List<LanguageModel> Words = new();

        public Random random = new();

        public MainPageViewModel(LanguageService languageService)
        {
            _languageService = languageService;
            Title = "Language Flashcard";
            InitWordsListAsync();
        }

        [ObservableProperty]
        public bool isVisible;

        [ObservableProperty]
        public string? foreignWord;

        [ObservableProperty]
        public string? englishWord;

        public async Task GetWordsAsync()
        {
            await GetRandomWord();
            await Task.Delay(3000);
            IsVisible = true;
        }

        public async Task GetRandomWord()
        {
            if (Words.Count <= 0)
            {
                var newWordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(newWordsList);
            }

            var randomWord = Words.ElementAt(random.Next(0, Words.Count));
            ForeignWord = randomWord.ForeignFormat;
            EnglishWord = randomWord.EnglishFormat;
        }

        public async Task NextCard()
        {
            await GetRandomWord();
            IsVisible = false;
            await Task.Delay(3000);
            IsVisible = true;
        }

        public async Task InitWordsListAsync()
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
                    await GetWordsAsync();
                    IsBusy = false;
                    return;
                }
                var wordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(wordsList);
                await GetWordsAsync();
                IsBusy = false;
            }
            catch (FileNotFoundException)
            {
                var wordsList = await ReadWordsFromAppDataFile();
                ParseWordsList(wordsList);
                await GetWordsAsync();
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
                Words!.Add(word);
        }

        public void RemoveWord(string? foreign)
        {
            LanguageModel wordToRemove = Words.FirstOrDefault(word => word.ForeignFormat == foreign)!;
            Words.Remove(wordToRemove!);
        }

        [RelayCommand]
        public async Task AddToWordsToLearnAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                LanguageModel newWordToLearn = new() { ForeignFormat = ForeignWord, EnglishFormat = EnglishWord };
                await _languageService.AddToWordsToLearn(newWordToLearn);
                RemoveWord(ForeignWord);
                await NextCard();
                IsBusy = false;
                return;
            }
            catch (System.Exception exp)
            {
                Debug.WriteLine($"Exception: {exp.Message}");
                IsBusy = false;
                return;
            }
        }

        [RelayCommand]
        public async Task KnownWordAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                RemoveWord(ForeignWord);
                await NextCard();
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


