namespace Winforms_MVP_VM.Views;

using System.ComponentModel.Design.Serialization;

public partial class EmploymentInfoView : UserControl, IEmploymentInfoView
{
    private TextBox jobTitleTextBox;
    private TextBox companyTextBox;
    private TextBox salaryTextBox;
    private Button nextButton;
    private Button cancelButton;

    public ViewModels.EmploymentInfoViewModel ViewModel { get; set; }

    public string JobTitle
    {
        get => jobTitleTextBox.Text;
        set => jobTitleTextBox.Text = value;
    }

    public string Company
    {
        get => companyTextBox.Text;
        set => companyTextBox.Text = value;
    }

    public decimal Salary
    {
        get => decimal.TryParse(salaryTextBox.Text, out var result) ? result : 0;
        set => salaryTextBox.Text = value.ToString();
    }

    public event EventHandler NextButtonClicked;
    public event EventHandler CancelButtonClicked;

    public EmploymentInfoView()
    {
        InitializeComponentCustom();
    }

    private void InitializeComponentCustom()
    {
        SuspendLayout();
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(20)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

        var jobTitleLabel = new Label { Text = "Job Title:", AutoSize = true };
        jobTitleTextBox = new TextBox { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(jobTitleLabel, 0, 0);
        mainLayout.Controls.Add(jobTitleTextBox, 1, 0);

        var companyLabel = new Label { Text = "Company:", AutoSize = true };
        companyTextBox = new TextBox { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(companyLabel, 0, 1);
        mainLayout.Controls.Add(companyTextBox, 1, 1);

        var salaryLabel = new Label { Text = "Salary:", AutoSize = true };
        salaryTextBox = new TextBox { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(salaryLabel, 0, 2);
        mainLayout.Controls.Add(salaryTextBox, 1, 2);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        nextButton = new Button { Text = "Next", Width = 100, Height = 40 };
        cancelButton = new Button { Text = "Cancel", Width = 100, Height = 40};

        nextButton.Click += (s, e) => NextButtonClicked?.Invoke(this, EventArgs.Empty);
        cancelButton.Click += (s, e) => CancelButtonClicked?.Invoke(this, EventArgs.Empty);

        buttonPanel.Controls.Add(nextButton);
        buttonPanel.Controls.Add(cancelButton);
        mainLayout.Controls.Add(buttonPanel, 0, 3);
        mainLayout.SetColumnSpan(buttonPanel, 2);

        Controls.Add(mainLayout);
        ResumeLayout(false);
    }

    void IView<ViewModels.EmploymentInfoViewModel>.ShowView() { }
    void IView<ViewModels.EmploymentInfoViewModel>.CloseView() { }
}
