namespace Winforms_MVP_VM.Views;

public interface IPersonalInfoView : IView<ViewModels.PersonalInfoViewModel>
{
    event EventHandler CancelButtonClicked;
}
