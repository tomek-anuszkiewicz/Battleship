namespace BattleshipGame.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBattleshipGameCoreServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IConfig, Config>();
        serviceCollection.AddScoped<IRandomWrapper, RandomWrapper>();
        serviceCollection.AddScoped<IExitRequested, ExitRequested>();
        serviceCollection.AddScoped<IBoard, Board>();
        serviceCollection.AddScoped<ICommandsFactory, CommandsFactory>();

        serviceCollection.Scan(s => s.FromAssemblyOf<ICommand>()
            .AddClasses(c => c.AssignableTo<ICommand>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return serviceCollection;
    }
}