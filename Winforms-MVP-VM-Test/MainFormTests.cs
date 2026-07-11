namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.Models;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

[TestFixture]
public class MainFormTests
{
    [Test]
    public void CreateProfileButton_RaisesCreateProfileButtonClicked()
    {
        Sta.Run(() =>
        {
            var form = new MainForm();
            form.ViewModel = new MainViewModel();

            bool raised = false;
            form.CreateProfileButtonClicked += (s, e) => raised = true;
            ViewHelper.ClickButton(form, "createProfileButton");

            Assert.That(raised, Is.True);
        });
    }

    [Test]
    public void UpdateDataGrid_WithProfiles_DoesNotThrow()
    {
        Sta.Run(() =>
        {
            var form = new MainForm { AutoResizeParentForm = false };
            form.ViewModel = new MainViewModel();
            var profiles = new List<Profile> { new() { Name = "Alice" } };

            Assert.DoesNotThrow(() => form.UpdateDataGrid(profiles));
        });
    }

    private static void SimulateCellDoubleClick(MainForm form, int rowIndex)
    {
        var method = typeof(MainForm)
            .GetMethod("OnCellDoubleClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method!.Invoke(form, new object[] { form, new DataGridViewCellEventArgs(0, rowIndex) });
    }

    [Test]
    public void DoubleClick_ValidRowIndex_RaisesEditProfileRequested()
    {
        Sta.Run(() =>
        {
            var form = new MainForm { AutoResizeParentForm = false };
            var vm = new MainViewModel();
            var profile = new Profile { Name = "Alice" };
            vm.AddProfile(profile);
            form.ViewModel = vm;
            form.UpdateDataGrid(vm.Profiles);

            Profile? received = null;
            form.EditProfileRequested += (s, p) => received = p;
            SimulateCellDoubleClick(form, 0);

            Assert.That(received, Is.SameAs(profile));
        });
    }

    [Test]
    public void DoubleClick_HeaderRow_DoesNotRaiseEditProfileRequested()
    {
        Sta.Run(() =>
        {
            var form = new MainForm { AutoResizeParentForm = false };
            form.ViewModel = new MainViewModel();

            bool raised = false;
            form.EditProfileRequested += (s, p) => raised = true;
            SimulateCellDoubleClick(form, -1);

            Assert.That(raised, Is.False);
        });
    }
}