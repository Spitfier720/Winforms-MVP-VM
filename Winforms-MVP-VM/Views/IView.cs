namespace Winforms_MVP_VM.Views;

public interface IView<TViewModel> where TViewModel : class
{
    TViewModel ViewModel { get; set; }
    void ShowView();
    void CloseView();
}
