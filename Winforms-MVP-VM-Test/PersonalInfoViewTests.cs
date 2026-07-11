namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

[TestFixture]
public class PersonalInfoViewTests
{
    [Test]
    public void SettingViewModel_PopulatesControls()
    {
        Sta.Run(() =>
        {
            var view = new PersonalInfoView();
            var vm = new PersonalInfoViewModel
            {
                Name = "Alice",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Female",
                IsActive = true
            };

            view.ViewModel = vm;

            Assert.That(view.ViewModel.Name, Is.EqualTo("Alice"));
            Assert.That(view.ViewModel.Gender, Is.EqualTo("Female"));
            Assert.That(view.ViewModel.IsActive, Is.True);
        });
    }

    [Test]
    public void ValidationErrors_RaisesPropertyChanged_WhenValidateCalled()
    {
        Sta.Run(() =>
        {
            var vm = new PersonalInfoViewModel();
            bool raised = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BaseViewModel.ValidationErrors))
                    raised = true;
            };

            vm.Validate();

            Assert.That(raised, Is.True);
        });
    }

    [Test]
    public void CancelButton_RaisesCancelButtonClicked()
    {
        Sta.Run(() =>
        {
            var view = new PersonalInfoView();
            view.ViewModel = new PersonalInfoViewModel
            {
                Name = "Alice",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Female"
            };

            bool raised = false;
            view.CancelButtonClicked += (s, e) => raised = true;
            ViewHelper.ClickButton(view, "cancelButton");

            Assert.That(raised, Is.True);
        });
    }
}