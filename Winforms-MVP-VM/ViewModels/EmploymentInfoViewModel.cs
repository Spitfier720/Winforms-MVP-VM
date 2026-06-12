namespace Winforms_MVP_VM.ViewModels;

public class EmploymentInfoViewModel : BaseViewModel
{
    private string? jobTitle;
    private string? company;
    private decimal salary;

    public string? JobTitle
    {
        get => jobTitle;
        set
        {
            jobTitle = value;
            OnPropertyChanged();
        }
    }

    public string? Company
    {
        get => company;
        set
        {
            company = value;
            OnPropertyChanged();
        }
    }

    public decimal Salary
    {
        get => salary;
        set
        {
            salary = value;
            OnPropertyChanged();
        }
    }
}
