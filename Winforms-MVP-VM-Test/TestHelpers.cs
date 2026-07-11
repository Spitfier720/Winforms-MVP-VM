// Shared infrastructure used across all test files.

namespace Winforms_MVP_VM_Test;

using Moq;
using NUnit.Framework;
using Winforms_MVP_VM;
using Winforms_MVP_VM.Models;
using Winforms_MVP_VM.ViewModels;
using Winforms_MVP_VM.Views;

/// <summary>
/// Runs an action on a dedicated STA thread and rethrows any exception
/// on the calling thread. Required for all WinForms control tests.
/// Each call creates a fresh STA thread — do NOT share WinForms objects
/// across multiple Sta.Run() calls.
/// </summary>
internal static class Sta
{
    public static void Run(Action action)
    {
        Exception? caught = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }
}

/// <summary>
/// Locates a private Button field by name on a control and calls PerformClick().
/// </summary>
internal static class ViewHelper
{
    public static void ClickButton(Control control, string fieldName)
    {
        var field = control.GetType()
            .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var button = (Button)field!.GetValue(control)!;
        button.PerformClick();
    }
}
internal class TestMasterView : IMasterView
{
    public UserControl? LastDisplayedView { get; private set; }

    public void DisplayView(UserControl view)
    {
        LastDisplayedView = view;
    }
}

/// <summary>
/// Hand-written fake for IMainView that exposes event raisers for tests.
/// </summary>
internal class FakeMainView : IMainView
{
    public MainViewModel ViewModel { get; set; } = new();
    public List<Profile>? LastUpdatedProfiles { get; private set; }

    public event EventHandler? CreateProfileButtonClicked;
    public event NavigationEventHandler<Profile>? EditProfileRequested;

    public void RaiseCreate() => CreateProfileButtonClicked?.Invoke(this, EventArgs.Empty);
    public void RaiseEdit(Profile p) => EditProfileRequested?.Invoke(this, p);

    public void UpdateDataGrid(List<Profile> profiles) => LastUpdatedProfiles = profiles;
    public void ShowView() { }
    public void CloseView() { }
}