using Escola.Telas;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Escola.Entidade;
using System.Security.AccessControl;
using System.Data.SqlClient;
using System.Configuration;

namespace Escola
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    //Verificar se é possível unificar o botão de maximizar, minimizar e fechar em um local só para não ter que ficar repetindo em cada tela criada 


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
            AlunoUI alunoUI = new AlunoUI();
            alunoUI.Closed += (sender, e) =>
            {
                ListarAlunos();
            };
            alunoUI.Show();
        }
        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ButtonMaximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                ButtonMaximizar.Content = "🗖";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                ButtonMaximizar.Content = "🗗";
            }
        }

        private void DataGridAlunosInicializar(object sender, EventArgs e)
        {
            ListarAlunos();
        }
        private void ListarAlunos()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT id, Nome,Classe,DataNascimento FROM Alunos", connection);
                SqlDataReader reader = command.ExecuteReader();
                List<Aluno> alunos = new List<Aluno>();
                while (reader.Read())
                {
                    Aluno aluno = new Aluno
                    {
                        Id = reader.GetGuid(0),
                        Nome = reader.GetString(1),
                        Classe = reader.GetString(2),
                        DataNascimento = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                    };
                    alunos.Add(aluno); 
                }
                DataGridAlunos.ItemsSource = alunos;
            }
        }
        private void ButtonBuscarAluno_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxBusca.Text.Trim().Count() > 0)
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("SELECT Nome,Classe,DataNascimento FROM Alunos WHERE Nome LIKE @Nome", connection);
                        command.Parameters.AddWithValue("@Nome", "%" + TextBoxBusca.Text + "%");
                        SqlDataReader reader = command.ExecuteReader();
                        List<Aluno> busca = new List<Aluno>();
                        while (reader.Read())
                        {
                            Aluno aluno = new Aluno
                            {
                                Nome = reader.GetString(0),
                                Classe = reader.GetString(1),
                                DataNascimento = reader.GetDateTime(2),
                            };
                            busca.Add(aluno);
                        }
                        DataGridAlunos.ItemsSource = busca;
                        if (busca.Count == 0)
                        {
                            TextRodape.Text = "Aluno nao cadastrado!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao buscar aluno: " + ex.Message);
                }
            }
        }
        private void ApagarAluno_Click(object sender, RoutedEventArgs e)
        {
            var alunoSelecionado = DataGridAlunos.SelectedItem as Aluno;
            if (alunoSelecionado != null)
            {
                TextRodape.Text = "";
                var result = MessageBox.Show("Deseja realmente apagar o cadastro do aluno?", "Confirmação", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    try 
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand("DELETE FROM Alunos WHERE ID = @Id", connection);
                            command.Parameters.AddWithValue("@Id", alunoSelecionado.Id);
                            command.ExecuteNonQuery();
                        }
                        ListarAlunos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao apagar aluno: " + ex.Message);
                    }
                }
            }
            else { TextRodape.Text = "Selecione um aluno para apagar!"; }
        }
        private void AbrirAluno(object sender, MouseButtonEventArgs e)
        {
            var alunoSelecionado = DataGridAlunos.SelectedItem as Aluno;
            AlunoUI alunoUI = new AlunoUI(alunoSelecionado);
            alunoUI.Closed += (sender, e) =>
            {
                ListarAlunos();
            };
            alunoUI.Show();
        }
    }
}