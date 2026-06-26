namespace Winforms_MVP_VM.ViewModels;

public class EmploymentInfoViewModel : BaseViewModel
{
    private string? jobTitle;
    private string? company;
    private decimal salary;

    public event NavigationEventHandler<EmploymentInfoViewModel>? NextRequested;

    public string? JobTitle
    {
        get => jobTitle;
        set { jobTitle = value; OnPropertyChanged(); }
    }

    public string? Company
    {
        get => company;
        set { company = value; OnPropertyChanged(); }
    }

    public decimal Salary
    {
        get => salary;
        set { salary = value; OnPropertyChanged(); }
    }

    public void RequestNext()
    {
        if (Validate())
            NextRequested?.Invoke(this, this);
    }

    protected override void OnValidate(Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(JobTitle))
            errors[nameof(JobTitle)] = "Job title is required.";

        if (string.IsNullOrWhiteSpace(Company))
            errors[nameof(Company)] = "Company is required.";

        if (Salary <= 0)
            errors[nameof(Salary)] = "Salary must be greater than zero.";
    }
}