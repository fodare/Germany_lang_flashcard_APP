using flashcard.Model;
using flashcard.Services;
using flashcard.ViewModel;
using Moq;

namespace FlashcardTest
{
    public class MainPageViewModelTests
    {
        private Mock<ILanguageService> _languageServiceMock;
        private MainPageViewModel _cut;

        public MainPageViewModelTests()
        {
            _languageServiceMock = new Mock<ILanguageService>();
            _cut = new MainPageViewModel(_languageServiceMock.Object);

        }

        [Fact]
        public async Task GetWordsToLearn_ShallNotReturnEmpty_ListOfWords_ToLearn()
        {
            _languageServiceMock
                .SetupGet(mock => mock.ReadWordsToLearn().Result)
                .Returns(new List<LanguageModel> { new() { ForeignFormat = "ich", EnglishFormat = "I" }, new() { ForeignFormat = "bin", EnglishFormat = "am" } });

            var wordsTolearn = await _cut.GetWordsToLearn();
            string message = $"Words to learn list shall not be empty / null but it was {wordsTolearn.Count}";
            Assert.True(wordsTolearn.Any(), message);
        }

        [Fact]
        public async Task ReadWordsFromAppDataFile_ShallNotReturnEmpty_ListOfForeignWords()
        {
            _languageServiceMock
                .Setup(mock => mock.ReadWordsFromDataFile().Result)
                .Returns(new List<LanguageModel> { new() { ForeignFormat = "ich", EnglishFormat = "I" }, new() { ForeignFormat = "bin", EnglishFormat = "am" } });

            var words = await _cut.ReadWordsFromAppDataFile();
            string message = $"Words list shall not be empty / null but it was {words.Count}";
            Assert.True(words.Any(), message);
        }


        [InlineData("bin")]
        [InlineData("was")]
        [InlineData("hello")]
        [Theory]
        public void RemoveWord_RemovesGivenWord_FromWords(string foreignWord)
        {
            _cut.Words = new List<LanguageModel> {
                new() { ForeignFormat = "ich", EnglishFormat = "I" },
                new() { ForeignFormat = "bin", EnglishFormat = "am" },
                new() { ForeignFormat = "was", EnglishFormat = "what" },
                new() { ForeignFormat = "hallo", EnglishFormat = "hello" }
            };
            var wordToRemove = _cut.Words.Where(word => word.ForeignFormat == foreignWord).FirstOrDefault();
            _cut.RemoveWord(foreignWord);
            bool isWordPresentInWords = _cut.Words.Contains(wordToRemove!);
            string message = $"Expected {foreignWord} is deleted from word list but it's present";
            Assert.True(!isWordPresentInWords, message);
        }
    }
}