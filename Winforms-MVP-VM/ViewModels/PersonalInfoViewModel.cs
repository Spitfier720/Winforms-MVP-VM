namespace Winforms_MVP_VM.ViewModels;

public class PersonalInfoViewModel : BaseViewModel
{
    private string? name;
    private DateTime dateOfBirth = DateTime.Now;
    private string? gender;
    private bool isActive;

    public string? Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    public DateTime DateOfBirth
    {
        get => dateOfBirth;
        set
        {
            dateOfBirth = value;
            OnPropertyChanged();
        }
    }

    public string? Gender
    {
        get => gender;
        set
        {
            gender = value;
            OnPropertyChanged();
        }
    }

    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            OnPropertyChanged();
        }
    }
}
