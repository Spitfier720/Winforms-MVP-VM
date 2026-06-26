namespace Winforms_MVP_VM.Views;

public interface IContactInfoView : IView<ViewModels.ContactInfoViewModel>
{
    event EventHandler CancelButtonClicked;
}
