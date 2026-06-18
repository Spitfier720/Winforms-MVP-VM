namespace Winforms_MVP_VM.Presenters;

public class ProfileEditorPresenter : BasePresenter<Views.IMainView, ViewModels.MainViewModel>
{
    private readonly Views.BaseMasterView _masterForm;

    // Step views
    private Views.PersonalInfoView _personalInfoView;
    private Views.EmploymentInfoView _employmentInfoView;
    private Views.ContactInfoView _contactInfoView;

    // Step view models — owned by this presenter for the duration of the flow
    private ViewModels.PersonalInfoViewModel _personalInfoViewModel;
    private ViewModels.EmploymentInfoViewModel _employmentInfoViewModel;
    private ViewModels.ContactInfoViewModel _contactInfoViewModel;

    // The profile being created or edited
    private Models.Profile _currentProfile;
    private bool _isEditing;

    public ProfileEditorPresenter(Views.IMainView mainView, ViewModels.MainViewModel mainViewModel, Views.BaseMasterView masterForm)
        : base(mainView, mainViewModel)
    {
        _masterForm = masterForm;
    }

    protected override void SubscribeToViewEvents()
    {
        View.CreateProfileButtonClicked += OnCreateProfileClicked;
    }

    // -------------------------------------------------------------------------
    // Main view handlers
    // -------------------------------------------------------------------------

    private void OnCreateProfileClicked(object? sender, EventArgs e)
    {
        _isEditing = false;
        _currentProfile = new Models.Profile();
        ShowPersonalInfoView();
    }

    /// <summary>
    /// Call this from outside (e.g. when the user selects a row and clicks Edit)
    /// to begin editing an existing profile through the same multi-step flow.
    /// </summary>
    public void StartEdit(Models.Profile profile)
    {
        _isEditing = true;
        _currentProfile = profile;
        ShowPersonalInfoView();
    }

    // -------------------------------------------------------------------------
    // Step 1 — Personal Info
    // -------------------------------------------------------------------------

    private void ShowPersonalInfoView()
    {
        _personalInfoViewModel = new ViewModels.PersonalInfoViewModel
        {
            Name = _currentProfile.Name,
            DateOfBirth = _currentProfile.DateOfBirth,
            Gender = _currentProfile.Gender,
            IsActive = _currentProfile.IsActive
        };

        _personalInfoView = new Views.PersonalInfoView
        {
            ViewModel = _personalInfoViewModel
        };

        _personalInfoView.NextButtonClicked += OnPersonalInfoNext;
        _personalInfoView.CancelButtonClicked += OnCancelled;

        _masterForm.DisplayView(_personalInfoView);
    }

    private void OnPersonalInfoNext(object? sender, EventArgs e)
    {
        _currentProfile.Name = _personalInfoView.Name;
        _currentProfile.DateOfBirth = _personalInfoView.DateOfBirth;
        _currentProfile.Gender = _personalInfoView.Gender;
        _currentProfile.IsActive = _personalInfoView.IsActive;

        ShowEmploymentInfoView();
    }

    // -------------------------------------------------------------------------
    // Step 2 — Employment Info
    // -------------------------------------------------------------------------

    private void ShowEmploymentInfoView()
    {
        _employmentInfoViewModel = new ViewModels.EmploymentInfoViewModel
        {
            JobTitle = _currentProfile.JobTitle,
            Company = _currentProfile.Company,
            Salary = _currentProfile.Salary
        };

        _employmentInfoView = new Views.EmploymentInfoView
        {
            ViewModel = _employmentInfoViewModel
        };

        _employmentInfoView.NextButtonClicked += OnEmploymentInfoNext;
        _employmentInfoView.CancelButtonClicked += OnCancelled;

        _masterForm.DisplayView(_employmentInfoView);
    }

    private void OnEmploymentInfoNext(object? sender, EventArgs e)
    {
        _currentProfile.JobTitle = _employmentInfoView.JobTitle;
        _currentProfile.Company = _employmentInfoView.Company;
        _currentProfile.Salary = _employmentInfoView.Salary;

        ShowContactInfoView();
    }

    // -------------------------------------------------------------------------
    // Step 3 — Contact Info
    // -------------------------------------------------------------------------

    private void ShowContactInfoView()
    {
        _contactInfoViewModel = new ViewModels.ContactInfoViewModel
        {
            CellphoneNumber = _currentProfile.CellphoneNumber,
            Email = _currentProfile.Email
        };

        _contactInfoView = new Views.ContactInfoView
        {
            ViewModel = _contactInfoViewModel
        };

        _contactInfoView.SaveButtonClicked += OnSaved;
        _contactInfoView.CancelButtonClicked += OnCancelled;

        _masterForm.DisplayView(_contactInfoView);
    }

    private void OnSaved(object? sender, EventArgs e)
    {
        _currentProfile.CellphoneNumber = _contactInfoView.CellphoneNumber;
        _currentProfile.Email = _contactInfoView.Email;

        if (!_isEditing)
            ViewModel.AddProfile(_currentProfile);
        // If editing, the profile is already in the list and was mutated in place

        ShowMainView();
    }

    // -------------------------------------------------------------------------
    // Shared cancel — always returns to the main view without saving
    // -------------------------------------------------------------------------

    private void OnCancelled(object? sender, EventArgs e)
    {
        ShowMainView();
    }

    // -------------------------------------------------------------------------
    // Main view
    // -------------------------------------------------------------------------

    private void ShowMainView()
    {
        if (View is UserControl viewControl)
        {
            _masterForm.DisplayView(viewControl);
            View.UpdateDataGrid(ViewModel.Profiles);
        }
        else
        {
            var fallback = new UserControl();
            var mainDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            var createButton = new Button
            {
                Text = "Create Profile",
                Dock = DockStyle.Bottom,
                Height = 40,
                AutoSize = false
            };
            createButton.Click += (s, e) => OnCreateProfileClicked(s, e);
            mainDataGridView.DataSource = new BindingSource { DataSource = ViewModel.Profiles };
            fallback.Controls.Add(mainDataGridView);
            fallback.Controls.Add(createButton);
            fallback.Controls.SetChildIndex(createButton, 0);
            _masterForm.DisplayView(fallback);
        }
    }
}