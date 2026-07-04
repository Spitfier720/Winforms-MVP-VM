namespace Winforms_MVP_VM.Views;

public partial class EmploymentInfoView : UserControl, IEmploymentInfoView
{
    private TextBox jobTitleTextBox;
    private TextBox companyTextBox;
    private TextBox salaryTextBox;
    private Button nextButton;
    private Button cancelButton;

    private Label jobTitleErrorLabel;
    private Label companyErrorLabel;
    private Label salaryErrorLabel;

    public event EventHandler CancelButtonClicked;

    private ViewModels.EmploymentInfoViewModel _viewModel;
    public ViewModels.EmploymentInfoViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            jobTitleTextBox.Text = _viewModel.JobTitle;
            companyTextBox.Text = _viewModel.Company;
            salaryTextBox.Text = _viewModel.Salary.ToString();

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModels.EmploymentInfoViewModel.ValidationErrors))
                    UpdateErrorLabels();
            };
        }
    }

    public EmploymentInfoView()
    {
        InitializeComponentCustom();
    }

    private void UpdateErrorLabels()
    {
        var errors = _viewModel.ValidationErrors;
        jobTitleErrorLabel.Text = errors.TryGetValue("JobTitle", out var j) ? j : "";
        companyErrorLabel.Text = errors.TryGetValue("Company", out var c) ? c : "";
        salaryErrorLabel.Text = errors.TryGetValue("Salary", out var s) ? s : "";
    }

    private void InitializeComponentCustom()
    {
        SuspendLayout();
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 4,
            Padding = new Padding(20)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        // Job Title
        mainLayout.Controls.Add(new Label { Text = "Job Title:", AutoSize = true }, 0, 0);
        jobTitleTextBox = new TextBox { Dock = DockStyle.Fill };
        jobTitleTextBox.TextChanged += (s, e) => { if (_viewModel != null) _viewModel.JobTitle = jobTitleTextBox.Text; };
        mainLayout.Controls.Add(jobTitleTextBox, 1, 0);
        jobTitleErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(jobTitleErrorLabel, 2, 0);

        // Company
        mainLayout.Controls.Add(new Label { Text = "Company:", AutoSize = true }, 0, 1);
        companyTextBox = new TextBox { Dock = DockStyle.Fill };
        companyTextBox.TextChanged += (s, e) => { if (_viewModel != null) _viewModel.Company = companyTextBox.Text; };
        mainLayout.Controls.Add(companyTextBox, 1, 1);
        companyErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(companyErrorLabel, 2, 1);

        // Salary
        mainLayout.Controls.Add(new Label { Text = "Salary:", AutoSize = true }, 0, 2);
        salaryTextBox = new TextBox { Dock = DockStyle.Fill };
        salaryTextBox.TextChanged += (s, e) =>
        {
            if (_viewModel != null && decimal.TryParse(salaryTextBox.Text, out var result))
                _viewModel.Salary = result;
        };
        mainLayout.Controls.Add(salaryTextBox, 1, 2);
        salaryErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(salaryErrorLabel, 2, 2);

        // Buttons
        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        nextButton = new Button { Text = "Next", Width = 100, Height = 40 };
        cancelButton = new Button { Text = "Cancel", Width = 100, Height = 40 };
        nextButton.Click += (s, e) => _viewModel?.RequestNext();
        cancelButton.Click += (s, e) => CancelButtonClicked?.Invoke(this, EventArgs.Empty);
        buttonPanel.Controls.Add(nextButton);
        buttonPanel.Controls.Add(cancelButton);
        mainLayout.Controls.Add(buttonPanel, 0, 3);
        mainLayout.SetColumnSpan(buttonPanel, 3);

        Controls.Add(mainLayout);
        ResumeLayout(false);
    }

    // Test hook — allows tests to simulate a cancel click without UI interaction
    internal void OnCancelForTest() => CancelButtonClicked?.Invoke(this, EventArgs.Empty);

    void IView<ViewModels.EmploymentInfoViewModel>.ShowView() { }
    void IView<ViewModels.EmploymentInfoViewModel>.CloseView() { }
}