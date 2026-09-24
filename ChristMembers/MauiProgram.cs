using ChristMembers.Data;
using ChristMembers.Services;
using ChristMembers.ViewModels;
using Microsoft.Extensions.Logging;

namespace ChristMembers
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<MembroService>();
            builder.Services.AddTransient<MembrosViewModel>();
            builder.Services.AddSingleton<BancoDados>();
            return builder.Build();
        }
    }
}
