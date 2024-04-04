using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Visualizer8.Models.GraphDataPrimitives;
using Visualizer8.Models.Input;
using Visualizer8.Services;

namespace Visualizer8.ViewModel;

public partial class AddMicrotopicPopupModelView: ObservableObject
{
    private GraphId? _id = null;

    public GraphId? Id
    {
        get => _id;
        set
        {
            if (value is null) return;
            _id = value;

            var mc = _graphService.Raw.Microtopics.First(el => value == el.Id);

            MicrotopicName = mc.Name;

            if (mc.Parent is null) return;

            var topic = _graphService.Raw.Topics.First(t => t.Id == mc.Parent);

            if (topic.Parent is null)
            {
                Application.Current?.MainPage?.DisplayAlert("topic parent is null",
                    $"tell Aybjax that {topic.Id.Value} topics' parent is null", "OK");
                return;
            }

            var unit = _graphService.Raw.Units.First(u => u.Id == topic.Parent);

            SelectedUnit = new ComboItem(unit.Id, unit.Name, null);
            SelectedTopic = new ComboItem(topic.Id, topic.Name, null);
        }
    }
    
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

        GraphId id = Id ?? Guid.NewGuid();
        Microtopic = new NewMicrotopic(id, MicrotopicName, SelectedTopic.Id, null);
        MopupService.Instance.PopAsync();
    }
}