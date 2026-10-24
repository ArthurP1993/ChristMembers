using ChristMembers.Models;
using Supabase;

namespace ChristMembers.Services;

public class ResultadoConta
{
    public bool Sucesso { get; init; }

    public bool JaEntrou { get; init; }

    public string Mensagem { get; init; } = string.Empty;
}

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
            Client cliente = await _supabase.ObterAsync();

            var sessao = await cliente.Auth.SignIn(email.Trim(), senha);

            string? idAuth = sessao?.User?.Id;

            if (string.IsNullOrEmpty(idAuth))
            {
                await SairAsync();
                return "E-mail ou senha incorretos.";
            }

            string? erro = await CarregarPerfilAsync(cliente, idAuth);

            if (erro != null)
                await SairAsync();

            return erro;
        }
        catch (HttpRequestException)
        {
            await SairAsync();
            return "Não foi possível conectar. Verifique a internet.";
        }
        catch (Exception ex)
        {
            await SairAsync();

            if (ex.Message.Contains("not confirmed", StringComparison.OrdinalIgnoreCase))
                return "Confirme o seu e-mail antes de entrar. Procure o e-mail que enviamos.";

            return "E-mail ou senha incorretos.";
        }
    }

    public async Task<ResultadoConta> CriarContaAsync(string email, string senha)
    {
        try
        {
            Client cliente = await _supabase.ObterAsync();

            var sessao = await cliente.Auth.SignUp(email.Trim(), senha);

            string? idAuth = sessao?.User?.Id;

            // Confirmação de e-mail desligada: a conta já nasce com sessão.
            if (sessao != null
                && !string.IsNullOrEmpty(sessao.AccessToken)
                && !string.IsNullOrEmpty(idAuth))
            {
                string? erro = await CarregarPerfilAsync(cliente, idAuth);

                if (erro != null)
                {
                    await SairAsync();

                    return new ResultadoConta { Mensagem = erro };
                }

                return new ResultadoConta { Sucesso = true, JaEntrou = true };
            }

            return new ResultadoConta
            {
                Sucesso = true,
                Mensagem = "Enviamos um e-mail para confirmar o seu cadastro. " +
                           "Abra o link do e-mail e depois entre com a sua senha."
            };
        }
        catch (HttpRequestException)
        {
            return new ResultadoConta
            {
                Mensagem = "Não foi possível conectar. Verifique a internet."
            };
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already", StringComparison.OrdinalIgnoreCase))
            {
                return new ResultadoConta
                {
                    Mensagem = "Este e-mail já tem uma conta. Volte e use o botão Entrar."
                };
            }

            return new ResultadoConta
            {
                Mensagem = "Não foi possível criar a conta. Confira o e-mail e use uma senha com pelo menos 8 caracteres."
            };
        }
    }

    public async Task SairAsync()
    {
        UsuarioLogado = null;

        try
        {
            Client cliente = await _supabase.ObterAsync();

            await cliente.Auth.SignOut();
        }
        catch (Exception)
        {
            // Sem sessão ou sem internet: o importante é limpar o usuário local.
        }
    }

    // Lê o perfil e o setor. Devolve null se deu certo, ou a mensagem de erro.
    private async Task<string?> CarregarPerfilAsync(Client cliente, string idAuth)
    {
        var respostaPerfil = await cliente
            .From<PerfilUsuario>()
            .Where(p => p.IdAuth == idAuth)
            .Get();

        PerfilUsuario? perfil = respostaPerfil.Models.FirstOrDefault();

        if (perfil == null || !perfil.Ativo)
            return "Este acesso não está liberado. Fale com a administração.";

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
}