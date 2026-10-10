using System.Security.Cryptography;
using ChristMembers.Data;
using ChristMembers.Models;

namespace ChristMembers.Services;

public class AuthService
{
    public const string UsuarioAdmin = "admin";
    public const string CargoAdmin = "Administrador";

    private const int Iteracoes = 100_000;

    private readonly BancoDados _bancoDados;

    public Usuario? UsuarioLogado { get; private set; }

    public AuthService(BancoDados bancoDados)
    {
        _bancoDados = bancoDados;
    }

    public async Task<bool> ExisteUsuario()
    {
        return await _bancoDados.ExisteAlgumUsuario();
    }

    public async Task CriarUsuario(string nomeUsuario, string senha, string cargo)
    {
        byte[] sal = RandomNumberGenerator.GetBytes(16);

        Usuario usuario = new Usuario
        {
            NomeUsuario = Normalizar(nomeUsuario),
            Sal = Convert.ToBase64String(sal),
            SenhaHash = Convert.ToBase64String(GerarHash(senha, sal)),
            Cargo = cargo,
            Ativo = true,
            DataCriacao = DateTime.Now
        };

        await _bancoDados.SalvarUsuario(usuario);
    }

    public async Task<Usuario?> Validar(string nomeUsuario, string senha)
    {
        Usuario? usuario = await _bancoDados.BuscarUsuario(Normalizar(nomeUsuario));

        if (usuario == null || !usuario.Ativo)
            return null;

        byte[] esperado = Convert.FromBase64String(usuario.SenhaHash);
        byte[] calculado = GerarHash(senha, Convert.FromBase64String(usuario.Sal));

        if (!CryptographicOperations.FixedTimeEquals(esperado, calculado))
            return null;

        UsuarioLogado = usuario;

        return usuario;
    }

    public void Sair()
    {
        UsuarioLogado = null;
    }

    private static string Normalizar(string nomeUsuario)
    {
        return nomeUsuario.Trim().ToLowerInvariant();
    }

    private static byte[] GerarHash(string senha, byte[] sal)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            senha,
            sal,
            Iteracoes,
            HashAlgorithmName.SHA256,
            32);
    }
}