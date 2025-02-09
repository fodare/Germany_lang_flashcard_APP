using flashcard.ViewModel;
namespace flashcard.View
{
	public partial class MainPage : ContentPage
	{
		public MainPage(MainPageViewModel mainPageViewModel)
		{
			InitializeComponent();
			BindingContext = mainPageViewModel;
		}
	}
}