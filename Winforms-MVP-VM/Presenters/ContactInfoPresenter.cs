namespace Winforms_MVP_VM.Presenters;

public class ContactInfoPresenter : BasePresenter<Views.IContactInfoView, ViewModels.ContactInfoViewModel>
{
    public event EventHandler<ViewModels.ContactInfoViewModel> SaveClicked;
    public event EventHandler CancelClicked;

    public ContactInfoPresenter(Views.IContactInfoView view, ViewModels.ContactInfoViewModel viewModel)
        : base(view, viewModel)
    {
    }

    protected override void SubscribeToViewEvents()
    {
        View.SaveButtonClicked += OnSaveClicked;
        View.CancelButtonClicked += OnCancelClicked;
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        ViewModel.CellphoneNumber = View.CellphoneNumber;
        ViewModel.Email = View.Email;

        SaveClicked?.Invoke(this, ViewModel);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        CancelClicked?.Invoke(this, EventArgs.Empty);
    }
}
