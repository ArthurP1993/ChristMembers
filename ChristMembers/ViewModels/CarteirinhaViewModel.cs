using System.Globalization;
using ChristMembers.Models;

namespace ChristMembers.ViewModels;

public class DadoCarteirinha
{
    public string Rotulo { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;
}

public class CarteirinhaViewModel
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public string Nome { get; }

    public string Iniciais { get; }

    public string Numero { get; }

    public string MembroDesde { get; }

    public string TextoBatismo { get; }

    public Color CorFundoBatismo { get; }

    public Color CorTextoBatismo { get; }

    public List<DadoCarteirinha> Dados { get; } = new();

    public CarteirinhaViewModel(Membro membro)
    {
        Nome = membro.Nome;
        Iniciais = MontarIniciais(membro.Nome);
        Numero = $"Nº {membro.Id:D4}";

        DateTime? entrada = membro.DataEntrada;
        DateTime? nascimento = membro.DataNascimento;

        string desde = DataValida(entrada)
            ? $"Membro desde {entrada!.Value.ToString("MMMM 'de' yyyy", PtBr)}"
            : "Membro";

        MembroDesde = string.IsNullOrWhiteSpace(membro.NomeConsagracao)
            ? desde
            : $"{membro.NomeConsagracao} · {desde}";

        if (membro.FoiBatizado == true)
        {
            TextoBatismo = "Batizado";
            CorFundoBatismo = Color.FromArgb("#F8CE62");
            CorTextoBatismo = Color.FromArgb("#2B1D00");
        }
        else if (membro.FoiBatizado == false)
        {
            TextoBatismo = "Não batizado";
            CorFundoBatismo = Color.FromArgb("#33FFFFFF");
            CorTextoBatismo = Colors.White;
        }
        else
        {
            TextoBatismo = "Batismo não informado";
            CorFundoBatismo = Color.FromArgb("#33FFFFFF");
            CorTextoBatismo = Colors.White;
        }

        Adicionar("Telefone", membro.Telefone);
        Adicionar("CPF", membro.CPF);

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

        if (DataValida(entrada))
            Adicionar("Entrada na igreja", entrada!.Value.ToString("dd/MM/yyyy"));

        Adicionar("Consagração", membro.NomeConsagracao);
        Adicionar("Setor", membro.NomeSetor);

        Adicionar("Observação", membro.Observacao);
    }

    private void Adicionar(string rotulo, string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return;

        Dados.Add(new DadoCarteirinha
        {
            Rotulo = rotulo,
            Valor = valor.Trim()
        });
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

    private static string MontarIniciais(string nome)
    {
        string[] partes = nome
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

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
}