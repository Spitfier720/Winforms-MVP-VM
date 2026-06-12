namespace Winforms_MVP_VM.ViewModels;

public class ContactInfoViewModel : BaseViewModel
{
    private string? cellphoneNumber;
    private string? email;

    public string? CellphoneNumber
    {
        get => cellphoneNumber;
        set
        {
            cellphoneNumber = value;
            OnPropertyChanged();
        }
    }

    public string? Email
    {
        get => email;
        set
        {
            email = value;
            OnPropertyChanged();
        }
    }
}
