namespace Winforms_MVP_VM.Presenters;

public class PersonalInfoPresenter : BasePresenter<Views.IPersonalInfoView, ViewModels.PersonalInfoViewModel>
{
    public event EventHandler<ViewModels.PersonalInfoViewModel> NextClicked;
    public event EventHandler CancelClicked;

    public PersonalInfoPresenter(Views.IPersonalInfoView view, ViewModels.PersonalInfoViewModel viewModel)
        : base(view, viewModel)
    {
    }

    protected override void SubscribeToViewEvents()
    {
        View.NextButtonClicked += OnNextClicked;
        View.CancelButtonClicked += OnCancelClicked;
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
        ViewModel.Name = View.Name;
        ViewModel.DateOfBirth = View.DateOfBirth;
        ViewModel.Gender = View.Gender;
        ViewModel.IsActive = View.IsActive;

        NextClicked?.Invoke(this, ViewModel);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        CancelClicked?.Invoke(this, EventArgs.Empty);
    }
}
