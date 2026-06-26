namespace Winforms_MVP_VM.ViewModels;

public class PersonalInfoViewModel : BaseViewModel
{
    private string? name;
    private DateTime dateOfBirth = DateTime.Now;
    private string? gender;
    private bool isActive;

    public event NavigationEventHandler<PersonalInfoViewModel>? NextRequested;

    public string? Name
    {
        get => name;
        set { name = value; OnPropertyChanged(); }
    }

    public DateTime DateOfBirth
    {
        get => dateOfBirth;
        set { dateOfBirth = value; OnPropertyChanged(); }
    }

    public string? Gender
    {
        get => gender;
        set { gender = value; OnPropertyChanged(); }
    }

    public bool IsActive
    {
        get => isActive;
        set { isActive = value; OnPropertyChanged(); }
    }

    public void RequestNext()
    {
        if (Validate())
            NextRequested?.Invoke(this, this);
    }

    protected override void OnValidate(Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(Name))
            errors[nameof(Name)] = "Name is required.";

        if (DateOfBirth >= DateTime.Today)
            errors[nameof(DateOfBirth)] = "Date of birth must be in the past.";

        if (string.IsNullOrWhiteSpace(Gender))
            errors[nameof(Gender)] = "Gender is required.";
    }
}