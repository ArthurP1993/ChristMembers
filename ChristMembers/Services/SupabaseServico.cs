using ChristMembers.Config;
using Supabase;

namespace ChristMembers.Services;

public class SupabaseServico
{
    private readonly SemaphoreSlim _trava = new(1, 1);
    private bool _inicializado;

    public Client Cliente { get; }

    public SupabaseServico()
    {
        SupabaseOptions opcoes = new()
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false,
            SessionHandler = new SessaoSupabaseHandler()
        };

        Cliente = new Client(SupabaseConfig.Url, SupabaseConfig.ChavePublica, opcoes);
    }

    public async Task<Client> ObterAsync()
    {
        if (_inicializado)
            return Cliente;

        await _trava.WaitAsync();

        try
        {
            if (!_inicializado)
            {
                await Cliente.InitializeAsync();

                _inicializado = true;
            }
        }
        finally
        {
            _trava.Release();
        }

        return Cliente;
    }
}