using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChristMembers.Config;
using ChristMembers.Helpers;
using ChristMembers.Models;

namespace ChristMembers.ViewModels;

public class DadoCarteirinha
{
    public string Rotulo { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;
}

public class CarteirinhaViewModel : INotifyPropertyChanged
{
    private const string NaoInformado = "Não informado";

    private readonly Membro _membro;

    private string? _fotoUrl;

    // ---------- Cartão ----------

    public string NomeIgreja => IgrejaConfig.Nome;

    public string TituloCredencial => "Credencial de Membro";

    public string Cargo { get; }

    public bool TemCargo => !string.IsNullOrWhiteSpace(Cargo);

    public string Nome { get; }

    public string Iniciais { get; }

    public string Numero { get; }

    public string NascimentoTexto { get; }

    public string CpfTexto { get; }

    public List<DadoCarteirinha> CamposCartao { get; } = new();

    // ---------- Dados cadastrais (o botão abaixo do cartão) ----------

    public List<DadoCarteirinha> Dados { get; } = new();

    // ---------- Foto ----------

    public string? FotoUrl
    {
        get => _fotoUrl;

        set
        {
            if (_fotoUrl == value)
                return;

            _fotoUrl = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(TemFoto));
            OnPropertyChanged(nameof(SemFoto));
            OnPropertyChanged(nameof(TextoBotaoFoto));
        }
    }

    public bool TemFoto => !string.IsNullOrEmpty(_fotoUrl);

    public bool SemFoto => !TemFoto;

    public string TextoBotaoFoto =>
        string.IsNullOrWhiteSpace(_membro.NomeFoto) ? "Adicionar foto" : "Trocar foto";

    public CarteirinhaViewModel(Membro membro)
    {
        _membro = membro;

        Nome = membro.Nome;
        Iniciais = MontarIniciais(membro.Nome);
        Numero = $"Nº {membro.Id:D4}";

        Cargo = membro.NomeConsagracao?.Trim() ?? string.Empty;

        DateTime? nascimento = membro.DataNascimento;
        DateTime? entrada = membro.DataEntrada;
        DateTime? batismo = membro.DataBatismo;

        NascimentoTexto = DataValida(nascimento)
            ? nascimento!.Value.ToString("dd/MM/yyyy")
            : NaoInformado;

        CpfTexto = FormatarCpf(membro.CPF);

        // ----- Campos do cartão -----

        DateTime emissao = DataValida(membro.DataCadastro)
            ? membro.DataCadastro!.Value
            : DateTime.Today;

        AdicionarCampo("Emitida em", emissao.ToString("dd/MM/yyyy"));

        if (DataValida(entrada))
            AdicionarCampo("Data de membresia", entrada!.Value.ToString("dd/MM/yyyy"));

        if (DataValida(batismo))
            AdicionarCampo("Data de batismo", batismo!.Value.ToString("dd/MM/yyyy"));

        if (!string.IsNullOrWhiteSpace(membro.SiglaCongregacao))
            AdicionarCampo("Congregação", membro.SiglaCongregacao.Trim().ToUpperInvariant());

        AdicionarCampo("Telefone da igreja", IgrejaConfig.Telefone);

        // ----- Dados cadastrais -----

        Adicionar("Telefone", membro.Telefone);
        Adicionar("CPF", CpfTexto == NaoInformado ? string.Empty : CpfTexto);

        if (DataValida(nascimento))
        {
            Adicionar(
                "Nascimento",
                $"{nascimento!.Value:dd/MM/yyyy} ({CalcularIdade(nascimento.Value)} anos)");
        }

        Adicionar("E-mail", membro.Email);
        Adicionar("Endereço", MontarEndereco(membro));
        Adicionar("Bairro", membro.Bairro);
        Adicionar("Cidade", MontarCidade(membro));
        Adicionar("CEP", membro.CEP);
        Adicionar("Consagração", membro.NomeConsagracao);
        Adicionar("Setor", membro.NomeSetor);
        Adicionar("Congregação", membro.SiglaCongregacao);
        Adicionar("Gênero", membro.NomeGenero);
        Adicionar("Estado civil", membro.NomeEstadoCivil);

        if (membro.TemFilhos.HasValue)
            Adicionar("Filhos", membro.TemFilhos.Value ? "Sim" : "Não");

        if (DataValida(entrada))
            Adicionar("Filiação", entrada!.Value.ToString("dd/MM/yyyy"));

        if (DataValida(batismo))
            Adicionar("Data do batismo", batismo!.Value.ToString("dd/MM/yyyy"));

        if (membro.PossuiDeficiencia.HasValue)
            Adicionar("Deficiência", TextoDeficiencia(membro));

        Adicionar("Observação", membro.Observacao);
    }

    private void AdicionarCampo(string rotulo, string valor)
    {
        CamposCartao.Add(new DadoCarteirinha { Rotulo = rotulo, Valor = valor });
    }

    private void Adicionar(string rotulo, string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return;

        Dados.Add(new DadoCarteirinha { Rotulo = rotulo, Valor = valor.Trim() });
    }

    private static bool DataValida(DateTime? data)
    {
        return data.HasValue && data.Value.Year > 1900;
    }

    private static int CalcularIdade(DateTime nascimento)
    {
        DateTime hoje = DateTime.Today;

        int idade = hoje.Year - nascimento.Year;

        if (nascimento.Date > hoje.AddYears(-idade))
            idade--;

        return idade;
    }

    private static string FormatarCpf(string? cpf)
    {
        string d = CpfValidador.SomenteDigitos(cpf);

        if (d.Length == 0)
            return NaoInformado;

        return d.Length == 11
            ? $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}"
            : cpf!.Trim();
    }

    private static string MontarIniciais(string nome)
    {
        string[] partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (partes.Length == 0)
            return "?";

        if (partes.Length == 1)
            return partes[0][..1].ToUpperInvariant();

        return (partes[0][..1] + partes[^1][..1]).ToUpperInvariant();
    }

    private static string MontarEndereco(Membro membro)
    {
        List<string> partes = new();

        if (!string.IsNullOrWhiteSpace(membro.Endereco))
            partes.Add(membro.Endereco.Trim());

        if (!string.IsNullOrWhiteSpace(membro.Numero))
            partes.Add(membro.Numero.Trim());

        if (!string.IsNullOrWhiteSpace(membro.Complemento))
            partes.Add(membro.Complemento.Trim());

        return string.Join(", ", partes);
    }

    private static string MontarCidade(Membro membro)
    {
        bool temCidade = !string.IsNullOrWhiteSpace(membro.Cidade);
        bool temEstado = !string.IsNullOrWhiteSpace(membro.Estado);

        if (temCidade && temEstado)
            return $"{membro.Cidade.Trim()} - {membro.Estado.Trim()}";

        return temCidade ? membro.Cidade.Trim() : membro.Estado?.Trim() ?? string.Empty;
    }

    private static string TextoDeficiencia(Membro membro)
    {
        if (membro.PossuiDeficiencia != true)
            return "Não";

        if (!string.IsNullOrWhiteSpace(membro.DeficienciaOutros))
        {
            return string.IsNullOrWhiteSpace(membro.NomeDeficiencia)
                ? membro.DeficienciaOutros.Trim()
                : $"{membro.NomeDeficiencia}: {membro.DeficienciaOutros.Trim()}";
        }

        return string.IsNullOrWhiteSpace(membro.NomeDeficiencia) ? "Sim" : membro.NomeDeficiencia;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propriedade = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propriedade));
    }
}