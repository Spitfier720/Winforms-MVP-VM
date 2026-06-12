namespace Winforms_MVP_VM;

public partial class Form1 : Views.BaseMasterView
{
    private Presenters.MainPresenter mainPresenter;
    private Views.MainForm mainView;
    private ViewModels.MainViewModel mainViewModel;

    public Form1()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        InitializePresenter();
    }

    private void InitializePresenter()
    {
        mainViewModel = new ViewModels.MainViewModel();
        mainView = new Views.MainForm { ViewModel = mainViewModel };
        mainPresenter = new Presenters.MainPresenter(mainView, mainViewModel, this);

        DisplayView(mainView);
    }
}
