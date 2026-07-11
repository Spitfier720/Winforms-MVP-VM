namespace Winforms_MVP_VM_Test;

using NUnit.Framework;
using Winforms_MVP_VM.Models;
using Winforms_MVP_VM.ViewModels;

[TestFixture]
public class MainViewModelTests
{
    private MainViewModel _vm = null!;

    [SetUp]
    public void SetUp() => _vm = new MainViewModel();

    [Test]
    public void AddProfile_IncreasesProfilesCount()
    {
        _vm.AddProfile(new Profile { Name = "Alice" });

        Assert.That(_vm.Profiles, Has.Count.EqualTo(1));
    }

    [Test]
    public void AddProfile_RaisesPropertyChangedForProfiles()
    {
        string? raised = null;
        _vm.PropertyChanged += (s, e) => raised = e.PropertyName;

        _vm.AddProfile(new Profile());

        Assert.That(raised, Is.EqualTo("Profiles"));
    }

    [Test]
    public void Profiles_Set_RaisesPropertyChanged()
    {
        string? raised = null;
        _vm.PropertyChanged += (s, e) => raised = e.PropertyName;

        _vm.Profiles = new List<Profile>();

        Assert.That(raised, Is.EqualTo("Profiles"));
    }
}