namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.ViewModels;

[TestFixture]
public class PersonalInfoViewModelTests
{
    private PersonalInfoViewModel _vm = null!;

    [SetUp]
    public void SetUp() => _vm = new PersonalInfoViewModel();

    // --- Validation ---

    [Test]
    public void Validate_AllValid_ReturnsTrue()
    {
        _vm.Name = "Alice";
        _vm.DateOfBirth = DateTime.Today.AddYears(-25);
        _vm.Gender = "Female";

        Assert.That(_vm.Validate(), Is.True);
        Assert.That(_vm.ValidationErrors, Is.Empty);
    }

    [Test]
    public void Validate_EmptyName_AddsError()
    {
        _vm.Name = "";
        _vm.DateOfBirth = DateTime.Today.AddYears(-25);
        _vm.Gender = "Male";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Name"), Is.True);
    }

    [Test]
    public void Validate_FutureDateOfBirth_AddsError()
    {
        _vm.Name = "Alice";
        _vm.DateOfBirth = DateTime.Today.AddDays(1);
        _vm.Gender = "Female";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("DateOfBirth"), Is.True);
    }

    [Test]
    public void Validate_TodayDateOfBirth_AddsError()
    {
        _vm.Name = "Alice";
        _vm.DateOfBirth = DateTime.Today;
        _vm.Gender = "Female";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("DateOfBirth"), Is.True);
    }

    [Test]
    public void Validate_EmptyGender_AddsError()
    {
        _vm.Name = "Alice";
        _vm.DateOfBirth = DateTime.Today.AddYears(-25);
        _vm.Gender = "";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Gender"), Is.True);
    }

    // --- PropertyChanged ---

    [Test]
    public void Name_Set_RaisesPropertyChanged()
    {
        string? raised = null;
        _vm.PropertyChanged += (s, e) => raised = e.PropertyName;

        _vm.Name = "Bob";

        Assert.That(raised, Is.EqualTo("Name"));
    }

    [Test]
    public void Validate_RaisesPropertyChangedForValidationErrors()
    {
        bool raised = false;
        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(BaseViewModel.ValidationErrors))
                raised = true;
        };

        _vm.Validate();

        Assert.That(raised, Is.True);
    }

    // --- NextRequested event ---

    [Test]
    public void RequestNext_ValidData_RaisesNextRequested()
    {
        _vm.Name = "Alice";
        _vm.DateOfBirth = DateTime.Today.AddYears(-25);
        _vm.Gender = "Female";

        PersonalInfoViewModel? received = null;
        _vm.NextRequested += (s, vm) => received = vm;

        _vm.RequestNext();

        Assert.That(received, Is.SameAs(_vm));
    }

    [Test]
    public void RequestNext_InvalidData_DoesNotRaiseNextRequested()
    {
        _vm.Name = "";

        bool raised = false;
        _vm.NextRequested += (s, vm) => raised = true;

        _vm.RequestNext();

        Assert.That(raised, Is.False);
    }

    [Test]
    public void RequestNext_InvalidData_PopulatesValidationErrors()
    {
        _vm.Name = "";
        _vm.DateOfBirth = DateTime.Today.AddYears(-25);
        _vm.Gender = "Male";

        _vm.RequestNext();

        Assert.That(_vm.ValidationErrors.ContainsKey("Name"), Is.True);
    }
}