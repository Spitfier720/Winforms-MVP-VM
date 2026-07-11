namespace Winforms_MVP_VM.Views;

public partial class MainForm : UserControl, IMainView
{
    private Button createProfileButton;
    private DataGridView profileDataGrid;

    public ViewModels.MainViewModel ViewModel { get; set; }

    // When true the containing Form will be resized to fit the DataGridView contents after UpdateDataGrid is called.
    public bool AutoResizeParentForm { get; set; } = true;

    public event EventHandler CreateProfileButtonClicked;
    public event NavigationEventHandler<Models.Profile> EditProfileRequested;

    public MainForm()
    {
        InitializeComponentCustom();
    }

    private void InitializeComponentCustom()
    {
        SuspendLayout();
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;

        createProfileButton = new Button
        {
            Text = "Create Profile",
            Dock = DockStyle.Bottom,
            Height = 40,
            AutoSize = false
        };
        createProfileButton.Click += (s, e) => CreateProfileButtonClicked?.Invoke(this, EventArgs.Empty);

        profileDataGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        };
        profileDataGrid.CellDoubleClick += OnCellDoubleClick;

        Controls.Add(profileDataGrid);
        Controls.Add(createProfileButton);
        Controls.SetChildIndex(createProfileButton, 0);

        ResumeLayout(false);
    }

    private void OnCellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        // Ignore header row double-clicks
        if (e.RowIndex < 0) return;

        var profile = ViewModel?.Profiles?[e.RowIndex];
        if (profile != null)
            EditProfileRequested?.Invoke(this, profile);
    }

    public void UpdateDataGrid(List<Models.Profile> profiles)
    {
        profileDataGrid.DataSource = new BindingSource { DataSource = profiles };

        // Resize columns/rows to content so PreferredSize reflects real content size
        try
        {
            profileDataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            profileDataGrid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
            profileDataGrid.PerformLayout();
        }
        catch { }

        if (AutoResizeParentForm)
            AdjustParentFormSizeToContent();
    }

    private void AdjustParentFormSizeToContent()
    {
        var parentForm = FindForm();
        if (parentForm == null) return;

        parentForm.SuspendLayout();
        profileDataGrid.PerformLayout();

        var gridPreferred = profileDataGrid.PreferredSize;

        int desiredWidth = gridPreferred.Width + SystemInformation.VerticalScrollBarWidth;
        int desiredHeight = gridPreferred.Height + createProfileButton.Height + SystemInformation.CaptionHeight + 80;

        var screen = Screen.FromControl(parentForm).WorkingArea;
        desiredWidth = Math.Min(screen.Width, desiredWidth);
        desiredHeight = Math.Min(screen.Height, desiredHeight);

        parentForm.ClientSize = new Size(Math.Max(300, desiredWidth), Math.Max(200, desiredHeight));
        parentForm.ResumeLayout();
    }

    void IView<ViewModels.MainViewModel>.ShowView() => Show();
    void IView<ViewModels.MainViewModel>.CloseView() => Hide();
}