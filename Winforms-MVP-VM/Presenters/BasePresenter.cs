namespace Winforms_MVP_VM.Presenters;

public abstract class BasePresenter<TView, TViewModel>
    where TView : class
    where TViewModel : class
{
    protected TView View { get; set; }
    protected TViewModel ViewModel { get; set; }

    public BasePresenter(TView view, TViewModel viewModel)
    {
        View = view;
        ViewModel = viewModel;
        SubscribeToViewEvents();
    }

    protected abstract void SubscribeToViewEvents();
}
