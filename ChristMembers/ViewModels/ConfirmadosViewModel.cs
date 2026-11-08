using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ItemConfirmado
{
    public string Nome { get; init; } = string.Empty;

    public string Telefone { get; init; } = string.Empty;

    public string TextoPessoas { get; init; } = string.Empty;

    public string Tipo { get; init; } = string.Empty;

    public string TextoAcompanhantes { get; init; } = string.Empty;

    public bool TemAcompanhantes => !string.IsNullOrEmpty(TextoAcompanhantes);
}

public class ConfirmadosViewModel : INotifyPropertyChanged
{
    private readonly ConfirmacaoService _confirmacoes;
    private readonly AuthService _auth;

    private Novidade? _novidade;
    private string _resumo = string.Empty;

    public ObservableCollection<ItemConfirmado> Itens { get; } = new();

    public string Titulo => _novidade?.Titulo ?? string.Empty;

    public string Resumo
    {
        get => _resumo;
        private set => SetProperty(ref _resumo, value);
    }

    public ConfirmadosViewModel(ConfirmacaoService confirmacoes, AuthService auth)
    {
        _confirmacoes = confirmacoes;
        _auth = auth;
    }

    public void Definir(Novidade novidade)
    {
        _novidade = novidade;

        OnPropertyChanged(nameof(Titulo));
    }

    public async Task Inicializar()
    {
        if (_auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        if (_novidade == null)
            return;

        try
        {
            List<ConfirmacaoPresenca> lista = await _confirmacoes.ListarDaNovidadeAsync(_novidade.Id);
            List<ConfirmacaoAcompanhante> acompanhantes = await _confirmacoes.ListarTodosAcompanhantesAsync();

            HashSet<long> ids = lista.Select(c => c.Id).ToHashSet();

            Dictionary<long, List<string>> nomesPorConfirmacao = acompanhantes
                .Where(a => ids.Contains(a.IdConfirmacao))
                .GroupBy(a => a.IdConfirmacao)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(a => a.Id).Select(a => a.Nome).ToList());

            Itens.Clear();

            foreach (ConfirmacaoPresenca c in lista)
            {
                nomesPorConfirmacao.TryGetValue(c.Id, out List<string>? nomes);
                nomes ??= new List<string>();

                int pessoas = 1 + nomes.Count;

                string textoAcompanhantes = nomes.Count == 0
                    ? string.Empty
                    : (nomes.Count == 1 ? "Acompanhante: " : "Acompanhantes: ")
                      + string.Join(", ", nomes);

                Itens.Add(new ItemConfirmado
                {
                    Nome = c.Nome,
                    Telefone = FormatarTelefone(c.Telefone),
                    TextoPessoas = pessoas == 1 ? "1 pessoa" : $"{pessoas} pessoas",
                    Tipo = c.EhMembro ? "Membro" : "Visitante",
                    TextoAcompanhantes = textoAcompanhantes
                });
            }

            int totalAcompanhantes = nomesPorConfirmacao.Values.Sum(v => v.Count);
            int totalPessoas = lista.Count + totalAcompanhantes;
            int membros = lista.Count(c => c.EhMembro);
            int visitantes = lista.Count - membros;

            Resumo = lista.Count == 0
                ? "Ninguém confirmou ainda."
                : $"{totalPessoas} pessoa(s) · {lista.Count} confirmação(ões) · {totalAcompanhantes} acompanhante(s) · {membros} membro(s) · {visitantes} visitante(s)";
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Sem conexão",
                "Não foi possível carregar as confirmações. Verifique a internet.",
                "OK");
        }
    }

    private static string FormatarTelefone(string telefone)
    {
        string d = CpfValidador.SomenteDigitos(telefone);

        return d.Length switch
        {
            11 => $"({d[..2]}) {d[2..7]}-{d[7..]}",
            10 => $"({d[..2]}) {d[2..6]}-{d[6..]}",
            _ => telefone
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propriedade = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propriedade));
    }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? propriedade = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
            return false;

        campo = valor;
        OnPropertyChanged(propriedade);
        return true;
    }
}