namespace Winforms_MVP_VM.Views;

using System.ComponentModel.Design.Serialization;

public partial class ContactInfoView : UserControl, IContactInfoView
{
    private TextBox cellphoneTextBox;
    private TextBox emailTextBox;
    private Button saveButton;
    private Button cancelButton;

    public ViewModels.ContactInfoViewModel ViewModel { get; set; }

    public string CellphoneNumber
    {
        get => cellphoneTextBox.Text;
        set => cellphoneTextBox.Text = value;
    }

    public string Email
    {
        get => emailTextBox.Text;
        set => emailTextBox.Text = value;
    }

    public event EventHandler SaveButtonClicked;
    public event EventHandler CancelButtonClicked;

    public ContactInfoView()
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
            RowCount = 3,
            Padding = new Padding(20)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

        var cellphoneLabel = new Label { Text = "Cellphone Number:", AutoSize = true };
        cellphoneTextBox = new TextBox { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(cellphoneLabel, 0, 0);
        mainLayout.Controls.Add(cellphoneTextBox, 1, 0);

        var emailLabel = new Label { Text = "Email Address:", AutoSize = true };
        emailTextBox = new TextBox { Dock = DockStyle.Fill };
        mainLayout.Controls.Add(emailLabel, 0, 1);
        mainLayout.Controls.Add(emailTextBox, 1, 1);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        saveButton = new Button { Text = "Save", Width = 100, Height = 40 };
        cancelButton = new Button { Text = "Cancel", Width = 100, Height = 40};

        saveButton.Click += (s, e) => SaveButtonClicked?.Invoke(this, EventArgs.Empty);
        cancelButton.Click += (s, e) => CancelButtonClicked?.Invoke(this, EventArgs.Empty);

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);
        mainLayout.Controls.Add(buttonPanel, 0, 2);
        mainLayout.SetColumnSpan(buttonPanel, 2);

        Controls.Add(mainLayout);
        ResumeLayout(false);
    }

    void IView<ViewModels.ContactInfoViewModel>.ShowView() { }
    void IView<ViewModels.ContactInfoViewModel>.CloseView() { }
}
