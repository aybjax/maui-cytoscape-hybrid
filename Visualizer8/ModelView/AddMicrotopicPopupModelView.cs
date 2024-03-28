using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Visualizer8.Models.Input;
using Visualizer8.Services;

namespace Visualizer8.ViewModel;

public partial class AddMicrotopicPopupModelView: ObservableObject
{
    private readonly GraphService _graphService;
    public ObservableCollection<ComboItem> Units { get; set;}
    public ObservableCollection<ComboItem> Topics { get; set; } = new();
    
    [ObservableProperty]
    public ComboItem _selectedUnit;
    [ObservableProperty]
    public ComboItem? _selectedTopic;
    [ObservableProperty]
    public string? _microtopicName;
    
    public NewMicrotopic? Microtopic { get; set; }

    public AddMicrotopicPopupModelView(GraphService graphService)
    {
        _graphService = graphService;

        var selectedUnit = _graphService.Unit.UnitTree.First(); 
        Units = new(_graphService.Unit.UnitTree.Select(u => new ComboItem(u.Value.Id, u.Value.Name, null)));
        SelectedUnit = new ComboItem(selectedUnit.Value.Id, selectedUnit.Value.Name, null);
    }

    partial void OnSelectedUnitChanged(ComboItem? value)
    {
        if (value is null) return;
        var unit = _graphService.Unit.UnitTree.First(u => u.Value.Id == value.Id);
        // if (unit is null) return;
        var topics = unit.Topics.Select(t => new ComboItem(t.Value.Id, t.Value.Name, null));
        Topics.Clear();
        foreach (var topic in topics)
        {
            Topics.Add(topic);
        }
    }

    [RelayCommand]
    void onFormSubmitted()
    {
        if (SelectedTopic is null)
        {
            Application.Current?.MainPage?.DisplayAlert("No topic", "Please select a topic", "OK");
            return;
        }
        if (MicrotopicName is null)
        {
            Application.Current?.MainPage?.DisplayAlert("No name", "Please type a name", "OK");
            return;
        }

        Microtopic = new NewMicrotopic(Guid.NewGuid(), MicrotopicName, SelectedTopic.Id);
        MopupService.Instance.PopAsync();
    }
}