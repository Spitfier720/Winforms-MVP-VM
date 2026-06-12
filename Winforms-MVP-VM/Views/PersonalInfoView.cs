namespace Winforms_MVP_VM.Views;

using System.ComponentModel.Design.Serialization;

public partial class PersonalInfoView : UserControl, IPersonalInfoView
{
    private TextBox nameTextBox;
    private DateTimePicker dateOfBirthPicker;
    private ComboBox genderComboBox;
    private CheckBox isActiveCheckBox;
    private Button nextButton;
    private Button cancelButton;

    public ViewModels.PersonalInfoViewModel ViewModel { get; set; }

    public new string Name
    {
        get => nameTextBox.Text;
        set => nameTextBox.Text = value;
    }

    public DateTime DateOfBirth
    {
        get => dateOfBirthPicker.Value;
        set => dateOfBirthPicker.Value = value;
    }

    public string Gender
    {
        get => genderComboBox.SelectedItem?.ToString() ?? "";
        set => genderComboBox.SelectedItem = value;
    }

    public bool IsActive
    {
        get => isActiveCheckBox.Checked;
        set => isActiveCheckBox.Checked = value;
    }

    public event EventHandler NextButtonClicked;
    public event EventHandler CancelButtonClicked;

    public PersonalInfoView()
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
            RowCount = 5,
            Padding = new Padding(20)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

        var nameLabel = new Label { Text = "Name:", AutoSize = true };
        nameTextBox = new TextBox { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(nameLabel, 0, 0);
        mainLayout.Controls.Add(nameTextBox, 1, 0);

        var dobLabel = new Label { Text = "Date of Birth:", AutoSize = true };
        dateOfBirthPicker = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
        mainLayout.Controls.Add(dobLabel, 0, 1);
        mainLayout.Controls.Add(dateOfBirthPicker, 1, 1);

        var genderLabel = new Label { Text = "Gender:", AutoSize = true };
        genderComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        genderComboBox.Items.AddRange(new[] { "Male", "Female", "Other" });
        mainLayout.Controls.Add(genderLabel, 0, 2);
        mainLayout.Controls.Add(genderComboBox, 1, 2);

        var isActiveLabel = new Label { Text = "Is Active:", AutoSize = true };
        isActiveCheckBox = new CheckBox { AutoSize = true };
        mainLayout.Controls.Add(isActiveLabel, 0, 3);
        mainLayout.Controls.Add(isActiveCheckBox, 1, 3);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        nextButton = new Button { Text = "Next", Width = 100, Height = 40 };
        cancelButton = new Button { Text = "Cancel", Width = 100, Height = 40};

        nextButton.Click += (s, e) => NextButtonClicked?.Invoke(this, EventArgs.Empty);
        cancelButton.Click += (s, e) => CancelButtonClicked?.Invoke(this, EventArgs.Empty);

        buttonPanel.Controls.Add(nextButton);
        buttonPanel.Controls.Add(cancelButton);
        mainLayout.Controls.Add(buttonPanel, 0, 4);
        mainLayout.SetColumnSpan(buttonPanel, 2);

        Controls.Add(mainLayout);
        ResumeLayout(false);
    }

    void IView<ViewModels.PersonalInfoViewModel>.ShowView() { }
    void IView<ViewModels.PersonalInfoViewModel>.CloseView() { }
}
