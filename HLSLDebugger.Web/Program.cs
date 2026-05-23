using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HLSLDebugger.Core;
using HLSLDebugger.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HLSLDebugger.Components.Debugger>("#app");

builder.Services.AddSingleton(new HostOptionsService());
builder.Services.AddSingleton<FileDialogService>();
builder.Services.AddSingleton<ImageLibraryService>();
builder.Services.AddSingleton(sp => new DebuggerProgram(
    new DebuggerExecutionEngine(), sp.GetRequiredService<FileDialogService>()));

await builder.Build().RunAsync();
