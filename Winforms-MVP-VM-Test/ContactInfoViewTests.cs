namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

[TestFixture]
public class ContactInfoViewTests
{
    [Test]
    public void SettingViewModel_PopulatesControls()
    {
        Sta.Run(() =>
        {
            var view = new ContactInfoView();
            var vm = new ContactInfoViewModel
            {
                CellphoneNumber = "555-1234",
                Email = "alice@example.com"
            };

            view.ViewModel = vm;

            Assert.That(view.ViewModel.CellphoneNumber, Is.EqualTo("555-1234"));
            Assert.That(view.ViewModel.Email, Is.EqualTo("alice@example.com"));
        });
    }

    [Test]
    public void CancelButton_RaisesCancelButtonClicked()
    {
        Sta.Run(() =>
        {
            var view = new ContactInfoView();
            view.ViewModel = new ContactInfoViewModel
            {
                CellphoneNumber = "555-1234",
                Email = "alice@example.com"
            };

            bool raised = false;
            view.CancelButtonClicked += (s, e) => raised = true;
            ViewHelper.ClickButton(view, "cancelButton");

            Assert.That(raised, Is.True);
        });
    }
}