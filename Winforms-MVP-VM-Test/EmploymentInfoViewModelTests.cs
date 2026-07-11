namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.Models;
using Winforms_MVP_VM.ViewModels;

[TestFixture]
public class EmploymentInfoViewModelTests
{
    private EmploymentInfoViewModel _vm = null!;

    [SetUp]
    public void SetUp() => _vm = new EmploymentInfoViewModel();

    [Test]
    public void Validate_AllValid_ReturnsTrue()
    {
        _vm.JobTitle = "Developer";
        _vm.Company = "Acme";
        _vm.Salary = 50000;

        Assert.That(_vm.Validate(), Is.True);
    }

    [Test]
    public void Validate_EmptyJobTitle_AddsError()
    {
        _vm.JobTitle = "";
        _vm.Company = "Acme";
        _vm.Salary = 50000;

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("JobTitle"), Is.True);
    }

    [Test]
    public void Validate_EmptyCompany_AddsError()
    {
        _vm.JobTitle = "Developer";
        _vm.Company = "";
        _vm.Salary = 50000;

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Company"), Is.True);
    }

    [Test]
    public void Validate_ZeroSalary_AddsError()
    {
        _vm.JobTitle = "Developer";
        _vm.Company = "Acme";
        _vm.Salary = 0;

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Salary"), Is.True);
    }

    [Test]
    public void Validate_NegativeSalary_AddsError()
    {
        _vm.JobTitle = "Developer";
        _vm.Company = "Acme";
        _vm.Salary = -1;

        _vm.Validate();

        Assert.That(_vm.ValidationErrors.ContainsKey("Salary"), Is.True);
    }

    [Test]
    public void RequestNext_ValidData_RaisesNextRequested()
    {
        _vm.JobTitle = "Developer";
        _vm.Company = "Acme";
        _vm.Salary = 50000;

        EmploymentInfoViewModel? received = null;
        _vm.NextRequested += (s, vm) => received = vm;

        _vm.RequestNext();

        Assert.That(received, Is.SameAs(_vm));
    }

    [Test]
    public void RequestNext_InvalidData_DoesNotRaiseNextRequested()
    {
        _vm.JobTitle = "";

        bool raised = false;
        _vm.NextRequested += (s, vm) => raised = true;

        _vm.RequestNext();

        Assert.That(raised, Is.False);
    }
}