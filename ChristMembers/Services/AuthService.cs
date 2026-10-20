using ChristMembers.Models;

namespace ChristMembers.Services;

public class AuthService
{
    public const string CargoAdmin = "Administrador";
    public const string CargoUsuario = "Usuário";

    private readonly SupabaseServico _supabase;

    public PerfilUsuario? UsuarioLogado { get; private set; }

    public AuthService(SupabaseServico supabase)
    {
        _supabase = supabase;
    }

    // Devolve null quando deu certo, ou a mensagem para mostrar na tela.
    public async Task<string?> EntrarAsync(string email, string senha)
    {
        try
        {
            var cliente = await _supabase.ObterAsync();

            var sessao = await cliente.Auth.SignIn(email.Trim(), senha);

            string? idAuth = sessao?.User?.Id;

            if (string.IsNullOrEmpty(idAuth))
            {
                await SairAsync();
                return "E-mail ou senha incorretos.";
            }

            var respostaPerfil = await cliente
                .From<PerfilUsuario>()
                .Where(p => p.IdAuth == idAuth)
                .Get();

            PerfilUsuario? perfil = respostaPerfil.Models.FirstOrDefault();

            if (perfil == null || !perfil.Ativo)
            {
                await SairAsync();
                return "Este acesso não está liberado. Fale com a administração.";
            }

            bool admin = false;

            if (perfil.IdSetor.HasValue)
            {
                long idSetor = perfil.IdSetor.Value;

                var respostaSetor = await cliente
                    .From<Setor>()
                    .Where(s => s.Id == idSetor)
                    .Get();

                admin = respostaSetor.Models.FirstOrDefault()?.Admin ?? false;
            }

            perfil.Cargo = admin ? CargoAdmin : CargoUsuario;

            UsuarioLogado = perfil;

            return null;
        }
        catch (HttpRequestException)
        {
            await SairAsync();
            return "Não foi possível conectar. Verifique a internet.";
        }
        catch (Exception)
        {
            await SairAsync();
            return "E-mail ou senha incorretos.";
        }
    }

    public async Task SairAsync()
    {
        UsuarioLogado = null;

        try
        {
            var cliente = await _supabase.ObterAsync();

            await cliente.Auth.SignOut();
        }
        catch (Exception)
        {
            // Sem sessão ou sem internet: o importante é limpar o usuário local.
        }
    }
}