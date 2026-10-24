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
            // Serviços
            builder.Services.AddSingleton<SupabaseServico>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<MembroService>();
            builder.Services.AddSingleton<VisitanteService>();
            builder.Services.AddSingleton<NovidadeService>();
            builder.Services.AddSingleton<PedidoOracaoService>();
            builder.Services.AddSingleton<ListasService>();

            // ViewModels
            builder.Services.AddTransient<MembrosViewModel>();
            builder.Services.AddTransient<VisitantesViewModel>();
            builder.Services.AddTransient<ListaMembrosViewModel>();
            builder.Services.AddTransient<ListaVisitantesViewModel>();
            builder.Services.AddTransient<EditarMembroViewModel>();
            builder.Services.AddTransient<NovidadesViewModel>();
            builder.Services.AddTransient<PublicarNovidadeViewModel>();
            builder.Services.AddTransient<NovoPedidoOracaoViewModel>();
            builder.Services.AddTransient<ListaPedidosOracaoViewModel>();
            builder.Services.AddTransient<MuralOracaoViewModel>();
            builder.Services.AddTransient<AniversariantesViewModel>();

            // Telas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<CriarContaPage>();
            builder.Services.AddTransient<PainelAdmin>();
            builder.Services.AddTransient<CadastroMembro>();
            builder.Services.AddTransient<CadastroVisitante>();
            builder.Services.AddTransient<CarteirinhaMembro>();
            builder.Services.AddTransient<EditarMembroPage>();
            builder.Services.AddTransient<ListaMembros>();
            builder.Services.AddTransient<ListaVisitantes>();
            builder.Services.AddTransient<NovidadesPage>();
            builder.Services.AddTransient<PublicarNovidadePage>();
            builder.Services.AddTransient<NovoPedidoOracaoPage>();
            builder.Services.AddTransient<ListaPedidosOracaoPage>();
            builder.Services.AddTransient<MuralOracaoPage>();
            builder.Services.AddTransient<AniversariantesPage>();

            return builder.Build();
        }
    }
}
