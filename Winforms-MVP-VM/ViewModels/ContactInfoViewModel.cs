namespace Winforms_MVP_VM.ViewModels;

public class ContactInfoViewModel : BaseViewModel
{
    private string? cellphoneNumber;
    private string? email;

    public event NavigationEventHandler<ContactInfoViewModel>? SaveRequested;

    public string? CellphoneNumber
    {
        get => cellphoneNumber;
        set { cellphoneNumber = value; OnPropertyChanged(); }
    }

    public string? Email
    {
        get => email;
        set { email = value; OnPropertyChanged(); }
    }

    public void RequestSave()
    {
        if (Validate())
            SaveRequested?.Invoke(this, this);
    }

    protected override void OnValidate(Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(CellphoneNumber))
            errors[nameof(CellphoneNumber)] = "Cellphone number is required.";

        if (string.IsNullOrWhiteSpace(Email))
            errors[nameof(Email)] = "Email is required.";
        else if (!Email.Contains('@'))
            errors[nameof(Email)] = "Email must be a valid address.";
    }
}