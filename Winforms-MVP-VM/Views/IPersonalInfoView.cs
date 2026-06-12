namespace Winforms_MVP_VM.Views;

public interface IPersonalInfoView : IView<ViewModels.PersonalInfoViewModel>
{
    event EventHandler NextButtonClicked;
    event EventHandler CancelButtonClicked;

    string Name { get; set; }
    DateTime DateOfBirth { get; set; }
    string Gender { get; set; }
    bool IsActive { get; set; }
}
