namespace Winforms_MVP_VM.Models;

public class Profile
{
    public string? Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public bool IsActive { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public decimal Salary { get; set; }
    public string? CellphoneNumber { get; set; }
    public string? Email { get; set; }

    public override string ToString()
    {
        return $"{Name} - {JobTitle} at {Company}";
    }
}
