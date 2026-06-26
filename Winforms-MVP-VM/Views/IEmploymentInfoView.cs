namespace Winforms_MVP_VM.Views;

public interface IEmploymentInfoView : IView<ViewModels.EmploymentInfoViewModel>
{
    event EventHandler CancelButtonClicked;
}
