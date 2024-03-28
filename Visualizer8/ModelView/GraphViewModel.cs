using System.Text;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visualizer8.Services;

namespace Visualizer8.ViewModel;

public partial class GraphModelView: ObservableObject
{
    private readonly GraphService _graphService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IFileSaver _fileSaver;

    public GraphModelView(GraphService graphService, IServiceProvider serviceProvider, IFileSaver fileSaver)
    {
        _graphService = graphService;
        _serviceProvider = serviceProvider;
        _fileSaver = fileSaver;
    }

    [RelayCommand]
    public async Task PickAndShow(PickOptions options)
    {
        var result = await FilePicker.Default.PickAsync(options);

        if (result is null)
        {
            Application.Current?.MainPage?.DisplayAlert("No file", "Please choose a file", "OK");
            return;
        }
        
        if (! result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
        {
            Application.Current?.MainPage?.DisplayAlert("No JSON", "Please choose a JSON file", "OK");
            return;
        }
        
        using var stream = await result.OpenReadAsync();
        // Read the source file into a byte array.
        byte[] bytes = new byte[stream.Length];
        int numBytesToRead = (int)stream.Length;
        int numBytesRead = 0;
        while (numBytesToRead > 0)
        {
            // Read may return anything from 0 to numBytesToRead.
            int n = stream.Read(bytes, numBytesRead, numBytesToRead);

            // Break when the end of the file is reached.
            if (n == 0)
                break;

            numBytesRead += n;
            numBytesToRead -= n;
        }

        var content = System.Text.Encoding.Default.GetString(bytes);

        _graphService.InitializeData(content);
    }
    
    

    [RelayCommand]
    public async Task PickAndSave(PickOptions options)
    {
        var json = _graphService.DumpJson();
        using var stream = new MemoryStream(Encoding.Default.GetBytes(json));

        try
        {
            var path = await _fileSaver.SaveAsync("knowledge-space.json",
                stream, default);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}