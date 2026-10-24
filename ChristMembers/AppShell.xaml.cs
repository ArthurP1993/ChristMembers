namespace ChristMembers;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(CadastroMembro), typeof(CadastroMembro));
        Routing.RegisterRoute(nameof(CadastroVisitante), typeof(CadastroVisitante));
        Routing.RegisterRoute(nameof(ListaMembros), typeof(ListaMembros));
        Routing.RegisterRoute(nameof(ListaVisitantes), typeof(ListaVisitantes));
        Routing.RegisterRoute(nameof(PainelAdmin), typeof(PainelAdmin));
        Routing.RegisterRoute(nameof(NovidadesPage), typeof(NovidadesPage));
        Routing.RegisterRoute(nameof(PublicarNovidadePage), typeof(PublicarNovidadePage));
        Routing.RegisterRoute(nameof(CarteirinhaMembro), typeof(CarteirinhaMembro));
        Routing.RegisterRoute(nameof(NovoPedidoOracaoPage), typeof(NovoPedidoOracaoPage));
        Routing.RegisterRoute(nameof(ListaPedidosOracaoPage), typeof(ListaPedidosOracaoPage));
        Routing.RegisterRoute(nameof(MuralOracaoPage), typeof(MuralOracaoPage));
        Routing.RegisterRoute(nameof(EditarMembroPage), typeof(EditarMembroPage));
        Routing.RegisterRoute(nameof(CriarContaPage), typeof(CriarContaPage));
    }
}