using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ResumoEvento
{
    public Novidade Novidade { get; init; } = null!;

    public string Titulo => Novidade.Titulo;

    public string TextoData { get; init; } = string.Empty;

    public string Situacao { get; init; } = string.Empty;

    public int Confirmacoes { get; init; }

    public int Pessoas { get; init; }

    public int Membros { get; init; }

    public int Visitantes { get; init; }

    public bool TemConfirmacoes => Confirmacoes > 0;

    public string TextoPessoas => Pessoas == 1 ? "1 pessoa" : $"{Pessoas} pessoas";

    public string TextoDetalhe => Confirmacoes == 0
        ? "Ninguém confirmou ainda"
        : $"{Confirmacoes} confirmação(ões) · {Membros} membro(s) · {Visitantes} visitante(s)";
}

public class PresencasViewModel : INotifyPropertyChanged
{
    private static readonly CultureInfo PtBr = new("pt-BR");
    private static readonly Color Azul = Color.FromArgb("#1B2F7A");
    private static readonly Color AzulClaro = Color.FromArgb("#E4EBFF");

    private readonly NovidadeService _novidades;
    private readonly ConfirmacaoService _confirmacoes;
    private readonly AuthService _auth;

    private List<ResumoEvento> _todos = new();
    private bool _mostrandoProximos = true;
    private string _resumo = string.Empty;

    public ObservableCollection<ResumoEvento> Itens { get; } = new();

    public Color CorFundoAbaProximos => _mostrandoProximos ? Azul : AzulClaro;
    public Color CorTextoAbaProximos => _mostrandoProximos ? Colors.White : Azul;
    public Color CorFundoAbaEncerrados => _mostrandoProximos ? AzulClaro : Azul;
    public Color CorTextoAbaEncerrados => _mostrandoProximos ? Azul : Colors.White;

    public string Resumo
    {
        get => _resumo;
        private set => SetProperty(ref _resumo, value);
    }

    public ICommand MostrarProximosCommand { get; }
    public ICommand MostrarEncerradosCommand { get; }

    public PresencasViewModel(
        NovidadeService novidades,
        ConfirmacaoService confirmacoes,
        AuthService auth)
    {
        _novidades = novidades;
        _confirmacoes = confirmacoes;
        _auth = auth;

        MostrarProximosCommand = new Command(() => TrocarAba(true));
        MostrarEncerradosCommand = new Command(() => TrocarAba(false));
    }

    public async Task Inicializar()
    {
        if (_auth.UsuarioLogado?.Cargo != AuthService.CargoAdmin)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        try
        {
            List<Novidade> eventos = await _novidades.ListarComConfirmacaoAsync();
            List<ConfirmacaoPresenca> confirmacoes = await _confirmacoes.ListarTodasAsync();

            Dictionary<long, List<ConfirmacaoPresenca>> porEvento = confirmacoes
                .GroupBy(c => c.IdNovidade)
                .ToDictionary(g => g.Key, g => g.ToList());

            DateTime hoje = DateTime.Today;

            _todos = eventos
                .Where(n => n.DataEvento.HasValue)
                .Select(n =>
                {
                    porEvento.TryGetValue(n.Id, out List<ConfirmacaoPresenca>? lista);
                    lista ??= new List<ConfirmacaoPresenca>();

                    DateTime dia = n.DataEvento!.Value.Date;

                    string situacao = dia == hoje
                        ? "Hoje"
                        : dia < hoje
                            ? "Encerrado"
                            : n.TextoEvento;

                    int membros = lista.Count(c => c.EhMembro);

                    return new ResumoEvento
                    {
                        Novidade = n,
                        TextoData = $"📅 {dia.ToString("dddd, dd/MM/yyyy", PtBr)}",
                        Situacao = situacao,
                        Confirmacoes = lista.Count,
                        Pessoas = lista.Sum(c => c.Pessoas),
                        Membros = membros,
                        Visitantes = lista.Count - membros
                    };
                })
                .ToList();
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Sem conexão",
                "Não foi possível carregar as confirmações. Verifique a internet.",
                "OK");
        }

        AplicarAba();
    }

    private void TrocarAba(bool proximos)
    {
        if (_mostrandoProximos == proximos)
            return;

        _mostrandoProximos = proximos;

        OnPropertyChanged(nameof(CorFundoAbaProximos));
        OnPropertyChanged(nameof(CorTextoAbaProximos));
        OnPropertyChanged(nameof(CorFundoAbaEncerrados));
        OnPropertyChanged(nameof(CorTextoAbaEncerrados));

        AplicarAba();
    }

    private void AplicarAba()
    {
        DateTime hoje = DateTime.Today;

        IEnumerable<ResumoEvento> lista = _mostrandoProximos
            ? _todos
                .Where(r => r.Novidade.DataEvento!.Value.Date >= hoje)
                .OrderBy(r => r.Novidade.DataEvento)
            : _todos
                .Where(r => r.Novidade.DataEvento!.Value.Date < hoje)
                .OrderByDescending(r => r.Novidade.DataEvento);

        List<ResumoEvento> itens = lista.ToList();

        Itens.Clear();

        foreach (ResumoEvento item in itens)
        {
            Itens.Add(item);
        }

        int pessoas = itens.Sum(i => i.Pessoas);

        string eventos = itens.Count == 1 ? "1 evento" : $"{itens.Count} eventos";
        string textoPessoas = pessoas == 1 ? "1 pessoa confirmada" : $"{pessoas} pessoas confirmadas";

        Resumo = $"{eventos} · {textoPessoas}";
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