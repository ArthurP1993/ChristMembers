using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChristMembers.Helpers;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class AniversariantesViewModel : INotifyPropertyChanged
{
    private static readonly CultureInfo PtBr = new("pt-BR");
    private static readonly Color Azul = Color.FromArgb("#1B2F7A");
    private static readonly Color AzulClaro = Color.FromArgb("#E4EBFF");

    private readonly MembroService _membroService;
    private readonly AuthService _auth;

    private bool _mostrandoSemana = true;
    private string _resumo = string.Empty;
    private string _avisoSemData = string.Empty;

    public ObservableCollection<Aniversariante> Itens { get; } = new();

    public bool EhAdmin => _auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin;

    public Color CorFundoAbaSemana => _mostrandoSemana ? Azul : AzulClaro;
    public Color CorTextoAbaSemana => _mostrandoSemana ? Colors.White : Azul;
    public Color CorFundoAbaMes => _mostrandoSemana ? AzulClaro : Azul;
    public Color CorTextoAbaMes => _mostrandoSemana ? Azul : Colors.White;

    public string Resumo
    {
        get => _resumo;
        private set => SetProperty(ref _resumo, value);
    }

    public string AvisoSemData
    {
        get => _avisoSemData;

        private set
        {
            if (SetProperty(ref _avisoSemData, value))
                OnPropertyChanged(nameof(TemAvisoSemData));
        }
    }

    public bool TemAvisoSemData => !string.IsNullOrEmpty(AvisoSemData);

    public ICommand MostrarSemanaCommand { get; }
    public ICommand MostrarMesCommand { get; }
    public ICommand ParabenizarCommand { get; }

    public AniversariantesViewModel(MembroService membroService, AuthService auth)
    {
        _membroService = membroService;
        _auth = auth;

        MostrarSemanaCommand = new Command(() => TrocarAba(true));
        MostrarMesCommand = new Command(() => TrocarAba(false));
        ParabenizarCommand = new Command<Aniversariante>(async a => await Parabenizar(a));
    }

    public async Task Inicializar()
    {
        if (!EhAdmin)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        try
        {
            await _membroService.Inicializar();
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert(
                "Sem conexão",
                "Não foi possível carregar os membros. Verifique a internet.",
                "OK");
        }

        Atualizar();
    }

    private void TrocarAba(bool semana)
    {
        if (_mostrandoSemana == semana)
            return;

        _mostrandoSemana = semana;

        OnPropertyChanged(nameof(CorFundoAbaSemana));
        OnPropertyChanged(nameof(CorTextoAbaSemana));
        OnPropertyChanged(nameof(CorFundoAbaMes));
        OnPropertyChanged(nameof(CorTextoAbaMes));

        Atualizar();
    }

    private void Atualizar()
    {
        DateTime hoje = DateTime.Today;

        List<Aniversariante> lista = _membroService.Membros
            .Select(m => Montar(m, hoje, _mostrandoSemana))
            .Where(a => a != null)
            .Cast<Aniversariante>()
            .ToList();

        IEnumerable<Aniversariante> ordenada = _mostrandoSemana
            ? lista.OrderBy(a => a.Dias).ThenBy(a => a.Nome)
            : lista.OrderBy(a => a.Data.Day).ThenBy(a => a.Nome);

        Itens.Clear();

        foreach (Aniversariante aniversariante in ordenada)
        {
            Itens.Add(aniversariante);
        }

        string quantidade = lista.Count == 1
            ? "1 aniversariante"
            : $"{lista.Count} aniversariantes";

        Resumo = _mostrandoSemana
            ? $"{quantidade} nos próximos 7 dias"
            : $"{quantidade} em {hoje.ToString("MMMM", PtBr)}";

        int semData = _membroService.Membros.Count(m => !TemDataValida(m));

        AvisoSemData = semData == 0
            ? string.Empty
            : semData == 1
                ? "1 membro está sem data de nascimento."
                : $"{semData} membros estão sem data de nascimento.";
    }

    private static bool TemDataValida(Membro membro)
    {
        return membro.DataNascimento.HasValue
            && membro.DataNascimento.Value.Year > 1900
            && membro.DataNascimento.Value.Date <= DateTime.Today;
    }

    private static Aniversariante? Montar(Membro membro, DateTime hoje, bool semana)
    {
        if (!TemDataValida(membro))
            return null;

        DateTime nascimento = membro.DataNascimento!.Value.Date;

        DateTime data = DiaNoAno(nascimento, hoje.Year);

        if (semana)
        {
            if (data < hoje)
                data = DiaNoAno(nascimento, hoje.Year + 1);

            if ((data - hoje).Days > 6)
                return null;
        }
        else if (nascimento.Month != hoje.Month)
        {
            return null;
        }

        return new Aniversariante
        {
            Nome = membro.Nome,
            Telefone = CpfValidador.SomenteDigitos(membro.Telefone),
            Data = data,
            Dias = (data - hoje).Days,
            Idade = data.Year - nascimento.Year
        };
    }

    // Quem nasceu em 29/02 comemora no dia 28 nos anos sem 29.
    private static DateTime DiaNoAno(DateTime nascimento, int ano)
    {
        int dia = nascimento.Day;

        if (nascimento.Month == 2 && dia == 29 && !DateTime.IsLeapYear(ano))
            dia = 28;

        return new DateTime(ano, nascimento.Month, dia);
    }

    private async Task Parabenizar(Aniversariante? aniversariante)
    {
        if (aniversariante == null || !EhAdmin || !aniversariante.PodeWhatsApp)
            return;

        string numero = aniversariante.Telefone;

        if (numero.Length is 10 or 11)
            numero = "55" + numero;

        string texto =
            $"Parabéns, {aniversariante.PrimeiroNome}! 🎉 " +
            "A igreja deseja um feliz aniversário, com muitas bênçãos de Deus!";

        try
        {
            await Launcher.Default.OpenAsync(
                new Uri($"https://wa.me/{numero}?text={Uri.EscapeDataString(texto)}"));
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Aviso", "Não foi possível abrir o WhatsApp.", "OK");
        }
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