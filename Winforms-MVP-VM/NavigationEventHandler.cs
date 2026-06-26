namespace Winforms_MVP_VM;

public delegate void NavigationEventHandler<TViewModel>(object sender, TViewModel viewModel)
    where TViewModel : class;