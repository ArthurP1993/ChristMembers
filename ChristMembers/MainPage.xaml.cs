using ChristMembers.Models;
using System.Collections.ObjectModel;

namespace ChristMembers;

public partial class MainPage : ContentPage
{
    private ObservableCollection<Membro> _membros = new ObservableCollection<Membro>();

    public MainPage()
    {
        InitializeComponent();

        ListaMembros.ItemsSource = _membros;
    }

    private void BtnCadastrar_Clicked(object sender, EventArgs e)
    {
        Membro membro = new Membro();

        membro.Nome = txtNome.Text;
        membro.Telefone = txtTelefone.Text;

        _membros.Add(membro);

        ClearFields();
    }

    private void ClearFields()
    {
        txtNome.Text = string.Empty;
        txtTelefone.Text = string.Empty;
    }
}