namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.ViewModels;

[TestFixture]
public class ContactInfoViewModelTests
{
    private ContactInfoViewModel _vm = null!;

    [SetUp]
    public void SetUp() => _vm = new ContactInfoViewModel();

    [Test]
    public void Validate_AllValid_ReturnsTrue()
    {
        _vm.CellphoneNumber = "555-1234";
        _vm.Email = "alice@example.com";

        Assert.That(_vm.Validate(), Is.True);
    }

    [Test]
    public void Validate_EmptyCellphone_AddsError()
    {
        _vm.CellphoneNumber = "";
        _vm.Email = "alice@example.com";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("CellphoneNumber"), Is.True);
    }

    [Test]
    public void Validate_EmptyEmail_AddsError()
    {
        _vm.CellphoneNumber = "555-1234";
        _vm.Email = "";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Email"), Is.True);
    }

    [Test]
    public void Validate_EmailMissingAtSign_AddsError()
    {
        _vm.CellphoneNumber = "555-1234";
        _vm.Email = "notanemail";

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Email"), Is.True);
    }

    [Test]
    public void RequestSave_ValidData_RaisesSaveRequested()
    {
        _vm.CellphoneNumber = "555-1234";
        _vm.Email = "alice@example.com";

        ContactInfoViewModel? received = null;
        _vm.SaveRequested += (s, vm) => received = vm;

        _vm.RequestSave();

        Assert.That(received, Is.SameAs(_vm));
    }

    [Test]
    public void RequestSave_InvalidData_DoesNotRaiseSaveRequested()
    {
        _vm.CellphoneNumber = "";

        bool raised = false;
        _vm.SaveRequested += (s, vm) => raised = true;

        _vm.RequestSave();

        Assert.That(raised, Is.False);
    }
}