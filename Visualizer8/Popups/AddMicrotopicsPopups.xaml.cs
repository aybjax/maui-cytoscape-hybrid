using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mopups.Pages;
using Visualizer8.Models.Input;
using Visualizer8.ViewModel;

namespace Visualizer8.Popups;

public partial class AddMicrotopicsPopups: PopupPage
{
    private readonly AddMicrotopicPopupModelView _mv;
    public AddMicrotopicsPopups(AddMicrotopicPopupModelView mv)
    {
        _mv = mv;
        InitializeComponent();
        BindingContext = _mv;
    }

    TaskCompletionSource<NewMicrotopic?>? _taskCompletionSource;
    public Task<NewMicrotopic?>? PopupDismissedTask =>  _taskCompletionSource?.Task;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _taskCompletionSource = new TaskCompletionSource<NewMicrotopic?>();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource?.SetResult(_mv.Microtopic);
    }
}