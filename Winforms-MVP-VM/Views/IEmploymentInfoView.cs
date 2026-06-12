namespace Winforms_MVP_VM.Views;

public interface IEmploymentInfoView : IView<ViewModels.EmploymentInfoViewModel>
{
    event EventHandler NextButtonClicked;
    event EventHandler CancelButtonClicked;

    string JobTitle { get; set; }
    string Company { get; set; }
    decimal Salary { get; set; }
}
