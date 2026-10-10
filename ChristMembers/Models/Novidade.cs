using SQLite;
using System.Globalization;

namespace ChristMembers.Models;

public class Novidade
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string NomeArquivoFoto { get; set; } = string.Empty;

    public DateTime DataPublicacao { get; set; }

    public DateTime? DataEvento { get; set; }

    [Ignore]
    public bool TemFoto => !string.IsNullOrEmpty(NomeArquivoFoto);

    [Ignore]
    public string CaminhoFoto => TemFoto
        ? Path.Combine(FileSystem.AppDataDirectory, "novidades", NomeArquivoFoto)
        : string.Empty;

    [Ignore]
    public bool Expirada =>
        DataEvento.HasValue && DataEvento.Value.Date < DateTime.Today;

    [Ignore]
    public bool TemDataEvento => DataEvento.HasValue;

    [Ignore]
    public string TextoEvento
    {
        get
        {
            if (!DataEvento.HasValue)
                return string.Empty;

            DateTime dia = DataEvento.Value.Date;

            if (dia == DateTime.Today)
                return "Hoje";

            if (dia == DateTime.Today.AddDays(1))
                return "Amanhã";

            return dia.ToString("dddd, dd/MM", new CultureInfo("pt-BR"));
        }
    }
}