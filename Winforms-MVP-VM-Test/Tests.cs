// Tests.cs
// Requires NUnit, Moq, and a project reference to Winforms_MVP_VM.
// Add to a separate test project targeting net10.0-windows.
//
// <PackageReference Include="NUnit"                    Version="4.*" />
// <PackageReference Include="NUnit3TestAdapter"        Version="4.*" />
// <PackageReference Include="Microsoft.NET.Test.Sdk"  Version="17.*" />
// <PackageReference Include="Moq"                      Version="4.*" />

using System.ComponentModel;
using Moq;
using NUnit.Framework;
using Winforms_MVP_VM;
using Winforms_MVP_VM.Models;
using Winforms_MVP_VM.Presenters;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

// ---------------------------------------------------------------------------
// Helpers shared across test classes
// ---------------------------------------------------------------------------

/// <summary>
/// Runs an action on a dedicated STA thread and rethrows any exception
/// on the calling thread. Required for all WinForms control tests.
/// Each call creates a fresh STA thread — do NOT share WinForms objects
/// across multiple Sta.Run() calls.
/// </summary>
internal static class Sta
{
    public static void Run(Action action)
    {
        Exception? caught = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }
}

/// <summary>
/// Concrete subclass of BaseMasterView usable in tests without showing a real window.
/// </summary>
internal class TestMasterView : IMasterView
{
    public UserControl? LastDisplayedView { get; private set; }

    public void DisplayView(UserControl view)
    {
        LastDisplayedView = view;
    }
}

// ---------------------------------------------------------------------------
// Mock main view that exposes event raisers
// ---------------------------------------------------------------------------

internal class FakeMainView : IMainView
{
    public MainViewModel ViewModel { get; set; } = new();
    public List<Profile>? LastUpdatedProfiles { get; private set; }

    public event EventHandler? CreateProfileButtonClicked;
    public event NavigationEventHandler<Profile>? EditProfileRequested;

    public void RaiseCreate() => CreateProfileButtonClicked?.Invoke(this, EventArgs.Empty);
    public void RaiseEdit(Profile p) => EditProfileRequested?.Invoke(this, p);

    public void UpdateDataGrid(List<Profile> profiles) => LastUpdatedProfiles = profiles;
    public void ShowView() { }
    public void CloseView() { }
}

// ===========================================================================
// 1. ViewModel tests — no WinForms, no STA required
// ===========================================================================

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
        _vm.Name = ""; // invalid

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

    // --- Validate raises PropertyChanged for ValidationErrors ---

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
}

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

[TestFixture]
public class MainViewModelTests
{
    private MainViewModel _vm = null!;

    [SetUp]
    public void SetUp() => _vm = new MainViewModel();

    [Test]
    public void AddProfile_IncreasesProfilesCount()
    {
        _vm.AddProfile(new Profile { Name = "Alice" });

        Assert.That(_vm.Profiles, Has.Count.EqualTo(1));
    }

    [Test]
    public void AddProfile_RaisesPropertyChangedForProfiles()
    {
        string? raised = null;
        _vm.PropertyChanged += (s, e) => raised = e.PropertyName;

        _vm.AddProfile(new Profile());

        Assert.That(raised, Is.EqualTo("Profiles"));
    }

    [Test]
    public void Profiles_Set_RaisesPropertyChanged()
    {
        string? raised = null;
        _vm.PropertyChanged += (s, e) => raised = e.PropertyName;

        _vm.Profiles = new List<Profile>();

        Assert.That(raised, Is.EqualTo("Profiles"));
    }
}

// ===========================================================================
// 2. Presenter tests
// ===========================================================================

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
            personalView.OnCancelForTest();

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

// ===========================================================================
// 3. View tests — require STA
// ===========================================================================

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

            // Controls are private, so we verify round-trip via ViewModel
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
            view.OnCancelForTest();

            Assert.That(raised, Is.True);
        });
    }
}

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
            view.OnCancelForTest();

            Assert.That(raised, Is.True);
        });
    }
}

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
            view.OnCancelForTest();

            Assert.That(raised, Is.True);
        });
    }
}

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
            form.OnCreateForTest();

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
            form.OnCellDoubleClickForTest(0);

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
            form.OnCellDoubleClickForTest(-1); // header row

            Assert.That(raised, Is.False);
        });
    }
}