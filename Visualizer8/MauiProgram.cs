using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Visualizer8.Popups;
using Visualizer8.Services;
using Visualizer8.ViewModel;

namespace Visualizer8;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); })
            .UseMauiCommunityToolkit()
            .ConfigureMopups();

        builder.Services.AddMauiBlazorWebView();
        
        //
        builder.Services.AddTransient<GraphModelView>();
        builder.Services.AddTransient<AddMicrotopicPopupModelView>();
        
        //
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AddMicrotopicsPopups>();
        
        //
        builder.Services.AddSingleton<GraphService>();
        builder.Services.AddTransient<JsService>();
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}