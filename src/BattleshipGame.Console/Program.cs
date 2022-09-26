using Microsoft.Extensions.DependencyInjection;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services
            .AddBattleshipGameCoreServices()
            .AddSingleton<IBoardDrawEngine, ConsoleBoardDrawEngine>()
            .AddSingleton<IConsoleWrapper, ConsoleWrapper>()
            .AddScoped<IMainGameLoop, MainGameLoop>()
            .AddSingleton<IConsoleColorMapper, ConsoleColorMapper>())
    .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;
var mainGameLoop = provider.GetRequiredService<IMainGameLoop>();

mainGameLoop.RunInLoop();