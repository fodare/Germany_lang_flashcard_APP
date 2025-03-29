using flashcard.Model;

namespace flashcard.Services
{
    public interface ILanguageService
    {
        Task<List<LanguageModel>> ReadWordsFromDataFile();
        Task<List<LanguageModel>> ReadWordsToLearn();
        Task<bool> WordInWordsToLearn(string foreignWord);
        Task AddToWordsToLearn(LanguageModel newWord);
    }
}