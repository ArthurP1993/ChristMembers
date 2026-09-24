using ChristMembers.Models;

namespace ChristMembers
{
    public partial class MainPage : ContentPage
    {
        private List<Membro> _membros = new List<Membro>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnCadastrar_Clicked(object sender, EventArgs e)
        {
            Membro membro = new Membro();

            membro.Nome = txtNome.Text;
            membro.Telefone = txtTelefone.Text;

            _membros.Add(membro);

            //ListaMembros.ItemsSource = null;
            //ListaMembros.ItemsSource = _membros;

            txtNome.Text = string.Empty;
            txtTelefone.Text = string.Empty;

            ClearFields();
        }

        private void ClearFields()
        {
            txtNome.Text = string.Empty;
            txtTelefone.Text = string.Empty;
        }
    }
}
