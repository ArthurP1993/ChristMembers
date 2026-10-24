using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.Helpers;

public static class AcessoHelper
{
    // paginasParaFechar: quantas telas (login, criar conta) saem da pilha ao entrar.
    public static async Task EntrarNaAreaAsync(
        AuthService auth,
        MembroService membros,
        int paginasParaFechar)
    {
        string voltar = string.Concat(Enumerable.Repeat("../", paginasParaFechar));

        if (auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin)
        {
            await Shell.Current.GoToAsync($"{voltar}{nameof(PainelAdmin)}");
            return;
        }

        string? idAuth = auth.UsuarioLogado?.IdAuth;

        Membro? meuCadastro = string.IsNullOrEmpty(idAuth)
            ? null
            : await membros.ObterPorAuthAsync(idAuth);

        if (meuCadastro == null)
        {
            await Shell.Current.GoToAsync($"{voltar}{nameof(CadastroMembro)}");
            return;
        }

        await Shell.Current.GoToAsync(
            $"{voltar}{nameof(CarteirinhaMembro)}",
            new ShellNavigationQueryParameters
            {
                { "Membro", meuCadastro },
                { "Proprio", true }
            });
    }
}