namespace Winforms_MVP_VM.Views;

public partial class PersonalInfoView : UserControl, IPersonalInfoView
{
    private TextBox nameTextBox;
    private DateTimePicker dateOfBirthPicker;
    private ComboBox genderComboBox;
    private CheckBox isActiveCheckBox;
    private Button nextButton;
    private Button cancelButton;

    private Label nameErrorLabel;
    private Label dobErrorLabel;
    private Label genderErrorLabel;

    public event EventHandler CancelButtonClicked;

    private ViewModels.PersonalInfoViewModel _viewModel;
    public ViewModels.PersonalInfoViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            nameTextBox.Text = _viewModel.Name;
            dateOfBirthPicker.Value = _viewModel.DateOfBirth < DateTimePicker.MinimumDateTime
                                              ? DateTimePicker.MinimumDateTime
                                              : _viewModel.DateOfBirth;
            genderComboBox.SelectedItem = _viewModel.Gender;
            isActiveCheckBox.Checked = _viewModel.IsActive;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModels.PersonalInfoViewModel.ValidationErrors))
                    UpdateErrorLabels();
            };
        }
    }

    public PersonalInfoView()
    {
        InitializeComponentCustom();
    }

    private void UpdateErrorLabels()
    {
        var errors = _viewModel.ValidationErrors;
        nameErrorLabel.Text = errors.TryGetValue("Name", out var n) ? n : "";
        dobErrorLabel.Text = errors.TryGetValue("DateOfBirth", out var d) ? d : "";
        genderErrorLabel.Text = errors.TryGetValue("Gender", out var g) ? g : "";
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
            RowCount = 5,
            Padding = new Padding(20)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        // Name
        mainLayout.Controls.Add(new Label { Text = "Name:", AutoSize = true }, 0, 0);
        nameTextBox = new TextBox { Dock = DockStyle.Fill };
        nameTextBox.TextChanged += (s, e) => { if (_viewModel != null) _viewModel.Name = nameTextBox.Text; };
        mainLayout.Controls.Add(nameTextBox, 1, 0);
        nameErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(nameErrorLabel, 2, 0);

        // Date of Birth
        mainLayout.Controls.Add(new Label { Text = "Date of Birth:", AutoSize = true }, 0, 1);
        dateOfBirthPicker = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
        dateOfBirthPicker.ValueChanged += (s, e) => { if (_viewModel != null) _viewModel.DateOfBirth = dateOfBirthPicker.Value; };
        mainLayout.Controls.Add(dateOfBirthPicker, 1, 1);
        dobErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(dobErrorLabel, 2, 1);

        // Gender
        mainLayout.Controls.Add(new Label { Text = "Gender:", AutoSize = true }, 0, 2);
        genderComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        genderComboBox.Items.AddRange(new[] { "Male", "Female", "Other" });
        genderComboBox.SelectedIndexChanged += (s, e) => { if (_viewModel != null) _viewModel.Gender = genderComboBox.SelectedItem?.ToString() ?? ""; };
        mainLayout.Controls.Add(genderComboBox, 1, 2);
        genderErrorLabel = new Label { ForeColor = Color.Red, AutoSize = true };
        mainLayout.Controls.Add(genderErrorLabel, 2, 2);

        // Is Active (no validation needed)
        mainLayout.Controls.Add(new Label { Text = "Is Active:", AutoSize = true }, 0, 3);
        isActiveCheckBox = new CheckBox { AutoSize = true };
        isActiveCheckBox.CheckedChanged += (s, e) => { if (_viewModel != null) _viewModel.IsActive = isActiveCheckBox.Checked; };
        mainLayout.Controls.Add(isActiveCheckBox, 1, 3);

        // Buttons
        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        nextButton = new Button { Text = "Next", Width = 100, Height = 40 };
        cancelButton = new Button { Text = "Cancel", Width = 100, Height = 40 };
        nextButton.Click += (s, e) => _viewModel?.RequestNext();
        cancelButton.Click += (s, e) => CancelButtonClicked?.Invoke(this, EventArgs.Empty);
        buttonPanel.Controls.Add(nextButton);
        buttonPanel.Controls.Add(cancelButton);
        mainLayout.Controls.Add(buttonPanel, 0, 4);
        mainLayout.SetColumnSpan(buttonPanel, 3);

        Controls.Add(mainLayout);
        ResumeLayout(false);
    }

    // Test hook — allows tests to simulate a cancel click without UI interaction
    internal void OnCancelForTest() => CancelButtonClicked?.Invoke(this, EventArgs.Empty);

    void IView<ViewModels.PersonalInfoViewModel>.ShowView() { }
    void IView<ViewModels.PersonalInfoViewModel>.CloseView() { }
}