namespace Winforms_MVP_VM.Views;

public interface IMainView : IView<ViewModels.MainViewModel>
{
    event EventHandler CreateProfileButtonClicked;
    event NavigationEventHandler<Models.Profile> EditProfileRequested;
    void UpdateDataGrid(List<Models.Profile> profiles);
}