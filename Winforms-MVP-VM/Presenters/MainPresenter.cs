namespace Winforms_MVP_VM.Presenters;

public class MainPresenter : BasePresenter<Views.IMainView, ViewModels.MainViewModel>
{
    private Views.PersonalInfoView personalInfoView;
    private Views.EmploymentInfoView employmentInfoView;
    private Views.ContactInfoView contactInfoView;

    private PersonalInfoPresenter personalInfoPresenter;
    private EmploymentInfoPresenter employmentInfoPresenter;
    private ContactInfoPresenter contactInfoPresenter;

    private Models.Profile currentProfile;
    private Views.BaseMasterView masterForm;

    public MainPresenter(Views.IMainView mainView, ViewModels.MainViewModel mainViewModel, Views.BaseMasterView masterForm)
        : base(mainView, mainViewModel)
    {
        this.masterForm = masterForm;
        currentProfile = new Models.Profile();
    }

    protected override void SubscribeToViewEvents()
    {
        View.CreateProfileButtonClicked += OnCreateProfileClicked;
    }

    private void OnCreateProfileClicked(object? sender, EventArgs e)
    {
        currentProfile = new Models.Profile();
        ShowPersonalInfoView();
    }

    private void ShowPersonalInfoView()
    {
        personalInfoView = new Views.PersonalInfoView();
        var viewModel = new ViewModels.PersonalInfoViewModel();
        personalInfoView.ViewModel = viewModel;

        personalInfoPresenter = new PersonalInfoPresenter(personalInfoView, viewModel);
        personalInfoPresenter.NextClicked += OnPersonalInfoNext;
        personalInfoPresenter.CancelClicked += OnPersonalInfoCancel;

        masterForm.DisplayView(personalInfoView);
    }

    private void OnPersonalInfoNext(object? sender, ViewModels.PersonalInfoViewModel viewModel)
    {
        currentProfile.Name = viewModel.Name;
        currentProfile.DateOfBirth = viewModel.DateOfBirth;
        currentProfile.Gender = viewModel.Gender;
        currentProfile.IsActive = viewModel.IsActive;

        ShowEmploymentInfoView();
    }

    private void OnPersonalInfoCancel(object? sender, EventArgs e)
    {
        ShowMainView();
    }

    private void ShowEmploymentInfoView()
    {
        employmentInfoView = new Views.EmploymentInfoView();
        var viewModel = new ViewModels.EmploymentInfoViewModel();
        employmentInfoView.ViewModel = viewModel;

        employmentInfoPresenter = new EmploymentInfoPresenter(employmentInfoView, viewModel);
        employmentInfoPresenter.NextClicked += OnEmploymentInfoNext;
        employmentInfoPresenter.CancelClicked += OnEmploymentInfoCancel;

        masterForm.DisplayView(employmentInfoView);
    }

    private void OnEmploymentInfoNext(object? sender, ViewModels.EmploymentInfoViewModel viewModel)
    {
        currentProfile.JobTitle = viewModel.JobTitle;
        currentProfile.Company = viewModel.Company;
        currentProfile.Salary = viewModel.Salary;

        ShowContactInfoView();
    }

    private void OnEmploymentInfoCancel(object? sender, EventArgs e)
    {
        ShowMainView();
    }

    private void ShowContactInfoView()
    {
        contactInfoView = new Views.ContactInfoView();
        var viewModel = new ViewModels.ContactInfoViewModel();
        contactInfoView.ViewModel = viewModel;

        contactInfoPresenter = new ContactInfoPresenter(contactInfoView, viewModel);
        contactInfoPresenter.SaveClicked += OnContactInfoSave;
        contactInfoPresenter.CancelClicked += OnContactInfoCancel;

        masterForm.DisplayView(contactInfoView);
    }

    private void OnContactInfoSave(object? sender, ViewModels.ContactInfoViewModel viewModel)
    {
        currentProfile.CellphoneNumber = viewModel.CellphoneNumber;
        currentProfile.Email = viewModel.Email;

        ViewModel.AddProfile(currentProfile);

        ShowMainView();
    }

    private void OnContactInfoCancel(object? sender, EventArgs e)
    {
        ShowMainView();
    }

    private void ShowMainView()
    {
        // Reuse the original main view so its layout and sizing logic are preserved
        // Ensure the view's ViewModel is set and the grid is updated
        if (View is UserControl viewControl)
        {
            // MUST attach the control to the parent Form first, otherwise FindForm() is null and sizing fails
            masterForm.DisplayView(viewControl);
            View.UpdateDataGrid(ViewModel.Profiles);
        }
        else
        {
            // Fallback: create a simple control if View is not a UserControl
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
            masterForm.DisplayView(fallback);
        }
    }
}
