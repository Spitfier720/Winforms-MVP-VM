namespace Winforms_MVP_VM.Views;

public partial class BaseMasterView : Form
{
    protected Panel containerPanel;

    public BaseMasterView()
    {
        InitializeComponent();
        SetupContainer();
    }

    private void SetupContainer()
    {
        containerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };
        Controls.Add(containerPanel);
    }

    public void DisplayView(UserControl view)
    {
        containerPanel.Controls.Clear();
        view.Dock = DockStyle.Fill;
        containerPanel.Controls.Add(view);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 600);
        Text = "Profile Manager";
        ResumeLayout(false);
    }
}
