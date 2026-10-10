using System.Collections.ObjectModel;
using ChristMembers.Models;
using ChristMembers.Services;

namespace ChristMembers.ViewModels;

public class ListaMembrosViewModel
{
    private readonly MembroService _membroService;

    public ObservableCollection<Membro> Membros => _membroService.Membros;

    public ListaMembrosViewModel(MembroService membroService)
    {
        _membroService = membroService;
    }

    public async Task Inicializar()
    {
        await _membroService.Inicializar();
    }
}