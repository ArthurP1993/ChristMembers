using System.Collections.ObjectModel;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class NovidadesViewModel
{
    private readonly NovidadeService _novidadeService;
    private readonly AuthService _auth;

    public ObservableCollection<Novidade> Novidades => _novidadeService.Novidades;

    public bool EhAdmin => _auth.UsuarioLogado?.Cargo == AuthService.CargoAdmin;

    public NovidadesViewModel(NovidadeService novidadeService, AuthService auth)
    {
        _novidadeService = novidadeService;
        _auth = auth;
    }

    public async Task Inicializar()
    {
        await _novidadeService.Inicializar();
    }
}