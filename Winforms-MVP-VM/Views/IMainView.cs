namespace Winforms_MVP_VM.Views;

public interface IMainView : IView<ViewModels.MainViewModel>
{
    event EventHandler CreateProfileButtonClicked;
    void UpdateDataGrid(List<Models.Profile> profiles);
}
