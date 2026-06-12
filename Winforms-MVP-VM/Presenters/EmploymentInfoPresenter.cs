namespace Winforms_MVP_VM.Presenters;

public class EmploymentInfoPresenter : BasePresenter<Views.IEmploymentInfoView, ViewModels.EmploymentInfoViewModel>
{
    public event EventHandler<ViewModels.EmploymentInfoViewModel> NextClicked;
    public event EventHandler CancelClicked;

    public EmploymentInfoPresenter(Views.IEmploymentInfoView view, ViewModels.EmploymentInfoViewModel viewModel)
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
        ViewModel.JobTitle = View.JobTitle;
        ViewModel.Company = View.Company;
        ViewModel.Salary = View.Salary;

        NextClicked?.Invoke(this, ViewModel);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        CancelClicked?.Invoke(this, EventArgs.Empty);
    }
}
