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
            builder.Services.AddTransient<CadastroMembro>();
            builder.Services.AddSingleton<VisitanteService>();
            builder.Services.AddTransient<VisitantesViewModel>();
            builder.Services.AddTransient<CadastroVisitante>();
            builder.Services.AddTransient<ListaMembrosViewModel>();
            builder.Services.AddTransient<ListaMembros>();
            builder.Services.AddTransient<ListaVisitantesViewModel>();
            builder.Services.AddTransient<ListaVisitantes>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<NovidadeService>();
            builder.Services.AddTransient<NovidadesViewModel>();
            builder.Services.AddTransient<NovidadesPage>();
            builder.Services.AddTransient<PublicarNovidadeViewModel>();
            builder.Services.AddTransient<PublicarNovidadePage>();

            return builder.Build();
        }
    }
}
