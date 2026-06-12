namespace Winforms_MVP_VM.Views;

public interface IContactInfoView : IView<ViewModels.ContactInfoViewModel>
{
    event EventHandler SaveButtonClicked;
    event EventHandler CancelButtonClicked;

    string CellphoneNumber { get; set; }
    string Email { get; set; }
}
