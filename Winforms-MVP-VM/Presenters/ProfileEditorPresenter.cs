namespace Winforms_MVP_VM.Presenters;

public class ProfileEditorPresenter : BasePresenter<Views.IMainView, ViewModels.MainViewModel>
{
    private readonly Views.BaseMasterView _masterForm;

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
        View.EditProfileRequested += OnEditProfileRequested;
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

    private void OnEditProfileRequested(object sender, Models.Profile profile)
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
        var view = new Views.PersonalInfoView
        {
            ViewModel = new ViewModels.PersonalInfoViewModel
            {
                Name = _currentProfile.Name,
                DateOfBirth = _currentProfile.DateOfBirth,
                Gender = _currentProfile.Gender,
                IsActive = _currentProfile.IsActive
            }
        };

        view.ViewModel.NextRequested += OnPersonalInfoNext;
        view.CancelButtonClicked += OnCancelled;

        _masterForm.DisplayView(view);
    }

    private void OnPersonalInfoNext(object sender, ViewModels.PersonalInfoViewModel viewModel)
    {
        _currentProfile.Name = viewModel.Name;
        _currentProfile.DateOfBirth = viewModel.DateOfBirth;
        _currentProfile.Gender = viewModel.Gender;
        _currentProfile.IsActive = viewModel.IsActive;

        ShowEmploymentInfoView();
    }

    // -------------------------------------------------------------------------
    // Step 2 — Employment Info
    // -------------------------------------------------------------------------

    private void ShowEmploymentInfoView()
    {
        var view = new Views.EmploymentInfoView
        {
            ViewModel = new ViewModels.EmploymentInfoViewModel
            {
                JobTitle = _currentProfile.JobTitle,
                Company = _currentProfile.Company,
                Salary = _currentProfile.Salary
            }
        };

        view.ViewModel.NextRequested += OnEmploymentInfoNext;
        view.CancelButtonClicked += OnCancelled;

        _masterForm.DisplayView(view);
    }

    private void OnEmploymentInfoNext(object sender, ViewModels.EmploymentInfoViewModel viewModel)
    {
        _currentProfile.JobTitle = viewModel.JobTitle;
        _currentProfile.Company = viewModel.Company;
        _currentProfile.Salary = viewModel.Salary;

        ShowContactInfoView();
    }

    // -------------------------------------------------------------------------
    // Step 3 — Contact Info
    // -------------------------------------------------------------------------

    private void ShowContactInfoView()
    {
        var view = new Views.ContactInfoView
        {
            ViewModel = new ViewModels.ContactInfoViewModel
            {
                CellphoneNumber = _currentProfile.CellphoneNumber,
                Email = _currentProfile.Email
            }
        };

        view.ViewModel.SaveRequested += OnSaved;
        view.CancelButtonClicked += OnCancelled;

        _masterForm.DisplayView(view);
    }

    private void OnSaved(object sender, ViewModels.ContactInfoViewModel viewModel)
    {
        _currentProfile.CellphoneNumber = viewModel.CellphoneNumber;
        _currentProfile.Email = viewModel.Email;

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