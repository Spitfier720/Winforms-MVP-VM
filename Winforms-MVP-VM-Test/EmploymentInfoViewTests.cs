namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

[TestFixture]
public class EmploymentInfoViewTests
{
    [Test]
    public void SettingViewModel_PopulatesControls()
    {
        Sta.Run(() =>
        {
            var view = new EmploymentInfoView();
            var vm = new EmploymentInfoViewModel
            {
                JobTitle = "Developer",
                Company = "Acme",
                Salary = 75000
            };

            view.ViewModel = vm;

            Assert.That(view.ViewModel.JobTitle, Is.EqualTo("Developer"));
            Assert.That(view.ViewModel.Company, Is.EqualTo("Acme"));
            Assert.That(view.ViewModel.Salary, Is.EqualTo(75000));
        });
    }

    [Test]
    public void CancelButton_RaisesCancelButtonClicked()
    {
        Sta.Run(() =>
        {
            var view = new EmploymentInfoView();
            view.ViewModel = new EmploymentInfoViewModel
            {
                JobTitle = "Developer",
                Company = "Acme",
                Salary = 50000
            };

            bool raised = false;
            view.CancelButtonClicked += (s, e) => raised = true;
            ViewHelper.ClickButton(view, "cancelButton");

            Assert.That(raised, Is.True);
        });
    }
}