namespace Winforms_MVP_VM.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Keyed by field name, value is the error message for that field
    private Dictionary<string, string> _validationErrors = new();
    public IReadOnlyDictionary<string, string> ValidationErrors => _validationErrors;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Runs validation. Returns true if the data is valid.
    /// Raises PropertyChanged for ValidationErrors so Views can react.
    /// </summary>
    public bool Validate()
    {
        _validationErrors = new Dictionary<string, string>();
        OnValidate(_validationErrors);
        OnPropertyChanged(nameof(ValidationErrors));
        return _validationErrors.Count == 0;
    }

    /// <summary>
    /// Override in each step ViewModel to populate errors by field name.
    /// </summary>
    protected virtual void OnValidate(Dictionary<string, string> errors) { }
}