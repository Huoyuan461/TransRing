using Avalonia.Controls;
using TransRing.App.Services;
using TransRing.App.ViewModels;

namespace TransRing.App.Views;

public partial class EngineSetupWindow : Window
{
    public EngineSetupWindow()
    {
        InitializeComponent();
        Motion.Track(this);
        DataContextChanged += (_, _) =>
        {
            if (DataContext is EngineSetupViewModel vm)
                vm.CloseRequested += (_, _) => Close();
        };
    }
}
