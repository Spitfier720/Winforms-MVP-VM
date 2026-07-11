namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.Models;
using Winforms_MVP_VM.Presenters;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

[TestFixture]
public class ProfileEditorPresenterTests
{
    // Builds a fresh presenter and its dependencies on the current thread.
    // Must be called from within an Sta.Run() block.
    private static (FakeMainView view, MainViewModel vm, TestMasterView master, ProfileEditorPresenter presenter)
        BuildPresenter()
    {
        var view = new FakeMainView();
        var vm = new MainViewModel();
        view.ViewModel = vm;
        var master = new TestMasterView();
        var presenter = new ProfileEditorPresenter(view, vm, master);
        return (view, vm, master, presenter);
    }

    // --- Create flow ---

    [Test]
    public void CreateProfileClicked_DisplaysPersonalInfoView()
    {
        Sta.Run(() =>
        {
            var (view, _, master, _) = BuildPresenter();
            view.RaiseCreate();
            Assert.That(master.LastDisplayedView, Is.InstanceOf<PersonalInfoView>());
        });
    }

    [Test]
    public void PersonalInfoNext_DisplaysEmploymentInfoView()
    {
        Sta.Run(() =>
        {
            var (view, _, master, _) = BuildPresenter();
            view.RaiseCreate();

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            personalView.ViewModel.Name = "Alice";
            personalView.ViewModel.DateOfBirth = DateTime.Today.AddYears(-25);
            personalView.ViewModel.Gender = "Female";
            personalView.ViewModel.RequestNext();

            Assert.That(master.LastDisplayedView, Is.InstanceOf<EmploymentInfoView>());
        });
    }

    [Test]
    public void EmploymentInfoNext_DisplaysContactInfoView()
    {
        Sta.Run(() =>
        {
            var (view, _, master, _) = BuildPresenter();
            view.RaiseCreate();

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            personalView.ViewModel.Name = "Alice";
            personalView.ViewModel.DateOfBirth = DateTime.Today.AddYears(-25);
            personalView.ViewModel.Gender = "Female";
            personalView.ViewModel.RequestNext();

            var employmentView = (EmploymentInfoView)master.LastDisplayedView!;
            employmentView.ViewModel.JobTitle = "Developer";
            employmentView.ViewModel.Company = "Acme";
            employmentView.ViewModel.Salary = 50000;
            employmentView.ViewModel.RequestNext();

            Assert.That(master.LastDisplayedView, Is.InstanceOf<ContactInfoView>());
        });
    }

    [Test]
    public void ContactInfoSave_AddsProfileToViewModel()
    {
        Sta.Run(() =>
        {
            var (view, vm, master, _) = BuildPresenter();
            view.RaiseCreate();

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            personalView.ViewModel.Name = "Alice";
            personalView.ViewModel.DateOfBirth = DateTime.Today.AddYears(-25);
            personalView.ViewModel.Gender = "Female";
            personalView.ViewModel.RequestNext();

            var employmentView = (EmploymentInfoView)master.LastDisplayedView!;
            employmentView.ViewModel.JobTitle = "Developer";
            employmentView.ViewModel.Company = "Acme";
            employmentView.ViewModel.Salary = 50000;
            employmentView.ViewModel.RequestNext();

            var contactView = (ContactInfoView)master.LastDisplayedView!;
            contactView.ViewModel.CellphoneNumber = "555-1234";
            contactView.ViewModel.Email = "alice@example.com";
            contactView.ViewModel.RequestSave();

            Assert.That(vm.Profiles, Has.Count.EqualTo(1));
            Assert.That(vm.Profiles[0].Name, Is.EqualTo("Alice"));
        });
    }

    [Test]
    public void ContactInfoSave_ProfileDataMatchesAllSteps()
    {
        Sta.Run(() =>
        {
            var (view, vm, master, _) = BuildPresenter();
            view.RaiseCreate();

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            personalView.ViewModel.Name = "Alice";
            personalView.ViewModel.DateOfBirth = new DateTime(1990, 6, 15);
            personalView.ViewModel.Gender = "Female";
            personalView.ViewModel.IsActive = true;
            personalView.ViewModel.RequestNext();

            var employmentView = (EmploymentInfoView)master.LastDisplayedView!;
            employmentView.ViewModel.JobTitle = "Developer";
            employmentView.ViewModel.Company = "Acme";
            employmentView.ViewModel.Salary = 75000;
            employmentView.ViewModel.RequestNext();

            var contactView = (ContactInfoView)master.LastDisplayedView!;
            contactView.ViewModel.CellphoneNumber = "555-1234";
            contactView.ViewModel.Email = "alice@example.com";
            contactView.ViewModel.RequestSave();

            var p = vm.Profiles[0];
            Assert.Multiple(() =>
            {
                Assert.That(p.Name, Is.EqualTo("Alice"));
                Assert.That(p.DateOfBirth, Is.EqualTo(new DateTime(1990, 6, 15)));
                Assert.That(p.Gender, Is.EqualTo("Female"));
                Assert.That(p.IsActive, Is.True);
                Assert.That(p.JobTitle, Is.EqualTo("Developer"));
                Assert.That(p.Company, Is.EqualTo("Acme"));
                Assert.That(p.Salary, Is.EqualTo(75000));
                Assert.That(p.CellphoneNumber, Is.EqualTo("555-1234"));
                Assert.That(p.Email, Is.EqualTo("alice@example.com"));
            });
        });
    }

    [Test]
    public void Cancel_OnPersonalInfo_ReturnsToMainView()
    {
        Sta.Run(() =>
        {
            var (view, _, master, _) = BuildPresenter();
            view.RaiseCreate();

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            ViewHelper.ClickButton(personalView, "cancelButton");

            Assert.That(master.LastDisplayedView, Is.Not.InstanceOf<PersonalInfoView>());
        });
    }

    // --- Edit flow ---

    [Test]
    public void EditProfileRequested_PrePopulatesPersonalInfoView()
    {
        Sta.Run(() =>
        {
            var (view, _, master, _) = BuildPresenter();
            var existing = new Profile
            {
                Name = "Bob",
                DateOfBirth = new DateTime(1985, 3, 10),
                Gender = "Male",
                IsActive = true
            };

            view.RaiseEdit(existing);

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            Assert.Multiple(() =>
            {
                Assert.That(personalView.ViewModel.Name, Is.EqualTo("Bob"));
                Assert.That(personalView.ViewModel.DateOfBirth, Is.EqualTo(new DateTime(1985, 3, 10)));
                Assert.That(personalView.ViewModel.Gender, Is.EqualTo("Male"));
                Assert.That(personalView.ViewModel.IsActive, Is.True);
            });
        });
    }

    [Test]
    public void EditFlow_Save_DoesNotAddDuplicateProfile()
    {
        Sta.Run(() =>
        {
            var (view, vm, master, _) = BuildPresenter();
            var existing = new Profile
            {
                Name = "Bob",
                DateOfBirth = new DateTime(1985, 3, 10),
                Gender = "Male",
                JobTitle = "Manager",
                Company = "Corp",
                Salary = 60000,
                CellphoneNumber = "555-9999",
                Email = "bob@example.com"
            };
            vm.AddProfile(existing);

            view.RaiseEdit(existing);

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            personalView.ViewModel.Name = "Bob Updated";
            personalView.ViewModel.DateOfBirth = new DateTime(1985, 3, 10);
            personalView.ViewModel.Gender = "Male";
            personalView.ViewModel.RequestNext();

            var employmentView = (EmploymentInfoView)master.LastDisplayedView!;
            employmentView.ViewModel.JobTitle = "Manager";
            employmentView.ViewModel.Company = "Corp";
            employmentView.ViewModel.Salary = 60000;
            employmentView.ViewModel.RequestNext();

            var contactView = (ContactInfoView)master.LastDisplayedView!;
            contactView.ViewModel.CellphoneNumber = "555-9999";
            contactView.ViewModel.Email = "bob@example.com";
            contactView.ViewModel.RequestSave();

            Assert.That(vm.Profiles, Has.Count.EqualTo(1));
            Assert.That(vm.Profiles[0].Name, Is.EqualTo("Bob Updated"));
        });
    }

    [Test]
    public void PersonalInfoNext_InvalidData_DoesNotAdvance()
    {
        Sta.Run(() =>
        {
            var (view, _, master, _) = BuildPresenter();
            view.RaiseCreate();

            var personalView = (PersonalInfoView)master.LastDisplayedView!;
            personalView.ViewModel.Name = "";
            personalView.ViewModel.DateOfBirth = DateTime.Today.AddYears(-25);
            personalView.ViewModel.Gender = "Male";
            personalView.ViewModel.RequestNext();

            Assert.That(master.LastDisplayedView, Is.SameAs(personalView));
        });
    }
}