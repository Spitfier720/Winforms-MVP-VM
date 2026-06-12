namespace Winforms_MVP_VM.ViewModels;

public class MainViewModel : BaseViewModel
{
    private List<Models.Profile> profiles = new();

    public List<Models.Profile> Profiles
    {
        get => profiles;
        set
        {
            profiles = value;
            OnPropertyChanged();
        }
    }

    public void AddProfile(Models.Profile profile)
    {
        profiles.Add(profile);
        OnPropertyChanged(nameof(Profiles));
    }
}
