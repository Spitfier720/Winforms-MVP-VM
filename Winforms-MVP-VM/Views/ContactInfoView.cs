namespace Winforms_MVP_VM.Views;

public partial class ContactInfoView : UserControl, IContactInfoView
{
    private TextBox cellphoneTextBox;
    private TextBox emailTextBox;
    private Button saveButton;
    private Button cancelButton;

    private Label cellphoneErrorLabel;
    private Label emailErrorLabel;

    public event EventHandler CancelButtonClicked;

    private ViewModels.ContactInfoViewModel _viewModel;
    public ViewModels.ContactInfoViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            cellphoneTextBox.Text = _viewModel.CellphoneNumber;
            emailTextBox.Text = _viewModel.Email;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModels.ContactInfoViewModel.ValidationErrors))
                    UpdateErrorLabels();
            };
        }
    }

    public ContactInfoView()
    {
        InitializeComponentCustom();
    }

    private void UpdateErrorLabels()
    {
        var errors = _viewModel.ValidationErrors;
        cellphoneErrorLabel.Text = errors.TryGetValue("CellphoneNumber", out var c) ? c : "";
        emailErrorLabel.Text = errors.TryGetValue("Email", out var e) ? e : "";
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
            RowCount = 3,
            Padding = new Padding(20)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        // Cellphone
        mainLayout.Controls.Add(new Label { Text = "Cellphone Number:", AutoSize = true }, 0, 0);
        cellphoneTextBox = new TextBox { Dock = DockStyle.Fill };
        cellphoneTextBox.TextChanged += (s, e) => { if (_viewModel != null) _viewModel.CellphoneNumber = cellphoneTextBox.Text; };
        mainLayout.Controls.Add(cellphoneTextBox, 1, 0);
        cellphoneErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(cellphoneErrorLabel, 2, 0);

        // Email
        mainLayout.Controls.Add(new Label { Text = "Email Address:", AutoSize = true }, 0, 1);
        emailTextBox = new TextBox { Dock = DockStyle.Fill };
        emailTextBox.TextChanged += (s, e) => { if (_viewModel != null) _viewModel.Email = emailTextBox.Text; };
        mainLayout.Controls.Add(emailTextBox, 1, 1);
        emailErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(emailErrorLabel, 2, 1);

        // Buttons
        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        saveButton = new Button { Text = "Save", Width = 100, Height = 40 };
        cancelButton = new Button { Text = "Cancel", Width = 100, Height = 40 };
        saveButton.Click += (s, e) => _viewModel?.RequestSave();
        cancelButton.Click += (s, e) => CancelButtonClicked?.Invoke(this, EventArgs.Empty);
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);
        mainLayout.Controls.Add(buttonPanel, 0, 2);
        mainLayout.SetColumnSpan(buttonPanel, 3);

        Controls.Add(mainLayout);
        ResumeLayout(false);
    }

    // Test hook — allows tests to simulate a cancel click without UI interaction
    internal void OnCancelForTest() => CancelButtonClicked?.Invoke(this, EventArgs.Empty);

    void IView<ViewModels.ContactInfoViewModel>.ShowView() { }
    void IView<ViewModels.ContactInfoViewModel>.CloseView() { }
}