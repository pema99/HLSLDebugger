using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using BlazorDesktop.Hosting;
using BlazorDesktop.Wpf;
using HLSLDebugger.Desktop;
using HLSLDebugger.Core;
using HLSLDebugger.Services;

var keyDownInfo = typeof(BlazorDesktopWindow)
    .GetMethod("WindowKeyDown", BindingFlags.Instance | BindingFlags.NonPublic);
if (keyDownInfo != null)
{
    EventManager.RegisterClassHandler(
        typeof(BlazorDesktopWindow),
        FrameworkElement.LoadedEvent,
        new RoutedEventHandler((sender, _) =>
        {
            var window = (BlazorDesktopWindow)sender;
            var handler = (KeyEventHandler)keyDownInfo.CreateDelegate(typeof(KeyEventHandler), window);
            window.KeyDown -= handler;
        }));
}

var builder = BlazorDesktopHostBuilder.CreateDefault(args);

builder.RootComponents.Add<HLSLDebugger.Components.Debugger>("#app");

string? initialCode = null;
string? initialName = null;
if (args.Length > 0 && File.Exists(args[0]))
{
    initialCode = File.ReadAllText(args[0]);
    initialName = Path.GetFileName(args[0]);
}
builder.Services.AddSingleton(new HostOptionsService
{
    InitialCode = initialCode,
    InitialName = initialName,
    InitialPath = args.Length > 0 ? args[0] : null,
    PermalinkUrl = "https://pema.dev/hlsl/",
    TabsEnabled = true,
});
builder.Services.AddSingleton<FileDialogService, WpfFileDialogService>();
builder.Services.AddSingleton<ImageLibraryService>();
builder.Services.AddSingleton(sp => new DebuggerProgram(
    new DebuggerExecutionEngine(), sp.GetRequiredService<FileDialogService>()));

builder.Window.UseTitle("HLSL Interpreter");
builder.Window.UseWidth(1600);
builder.Window.UseHeight(900);

if (builder.HostEnvironment.IsDevelopment())
{
    builder.UseDeveloperTools();
}

await builder.Build().RunAsync();
