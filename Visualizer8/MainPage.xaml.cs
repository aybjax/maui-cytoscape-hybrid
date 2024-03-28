using Visualizer8.ViewModel;

namespace Visualizer8;

public partial class MainPage : ContentPage
{
    public MainPage(GraphModelView mw)
    {
        InitializeComponent();
        BindingContext = mw;
    }
}