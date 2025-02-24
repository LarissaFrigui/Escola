using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Escola.Entidade;

namespace Escola.Telas
{
    /// <summary>
    /// Lógica interna para AlunoUI.xaml
    /// </summary>
    public partial class AlunoUI : Window
    {
        Aluno _alunoCadastrado = null;

        public AlunoUI()
        {
            InitializeComponent();
        }
        public AlunoUI(Aluno aluno)
        {
            _alunoCadastrado = aluno;
            InitializeComponent();
            TextBoxNomeAluno.Text = aluno.Nome; 
            TextBoxClasseAluno.Text = aluno.Classe;
            MaskedTextNascimentoAluno.Text = aluno.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty;
        }
        private void ButtonSalvarCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
            var dadosValidos = true;
            //testar o borderbrush
            try
            {
                if (string.IsNullOrWhiteSpace(TextBoxNomeAluno.Text))
                {
                    TextBoxNomeAluno.BorderBrush = Brushes.Red;
                    dadosValidos = false;
                }

                if (string.IsNullOrWhiteSpace(TextBoxClasseAluno.Text))
                {
                    TextBoxClasseAluno.BorderBrush = Brushes.Red;
                    dadosValidos = false;
                }

                if (string.IsNullOrWhiteSpace(MaskedTextNascimentoAluno.Text))
                {
                    MaskedTextNascimentoAluno.BorderBrush = Brushes.Red;
                    dadosValidos = false;
                }

                if (!dadosValidos)
                {
                    TextRodape.Text = "Dados incorretos, por favor verifique os campos sinalizados!";
                    return;
                }

                if (_alunoCadastrado == null)
                {
                    NovoAluno();
                }
                else
                {
                    Atualiza(_alunoCadastrado);
                }

                TextRodape.Text = "Salvo com sucesso!";

            }
            catch (Exception excecao)
            {
                Console.WriteLine(excecao);
                TextRodape.Text = "Erro inesperado! Por favor tente novamente mais tarde.";
            }
        }
        private void NovoAluno()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Aluno novoAluno = new Aluno();
                novoAluno.Id = Guid.NewGuid();
                novoAluno.Nome = TextBoxNomeAluno.Text;
                novoAluno.Classe = TextBoxClasseAluno.Text;
                if (DateTime.TryParseExact(MaskedTextNascimentoAluno.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataNascimento))
                {
                    novoAluno.DataNascimento = dataNascimento;
                }
                else
                {
                    TextRodape.Text = "Data inválida!";
                    return;
                }
                novoAluno.AlteradoEm = DateTime.Now;

                SqlCommand command = new SqlCommand("INSERT INTO Alunos (Id, Nome, Classe, DataNascimento, AlteradoEm) VALUES (@Id, @Nome, @Classe, @DataNascimento, @AlteradoEm)", connection);
                command.Parameters.AddWithValue("@Id", novoAluno.Id);
                command.Parameters.AddWithValue("@Nome", novoAluno.Nome);
                command.Parameters.AddWithValue("@Classe", novoAluno.Classe);
                command.Parameters.AddWithValue("@DataNascimento", novoAluno.DataNascimento);
                command.Parameters.AddWithValue("@AlteradoEm", novoAluno.AlteradoEm);
                command.ExecuteNonQuery();
                _alunoCadastrado = novoAluno;
            }
        }
        private void Atualiza(Aluno aluno)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                aluno.Nome = TextBoxNomeAluno.Text;
                aluno.Classe = TextBoxClasseAluno.Text;
                if (DateTime.TryParseExact(MaskedTextNascimentoAluno.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataNascimento))
                {
                    aluno.DataNascimento = dataNascimento;
                }
                else
                {
                    TextRodape.Text = "Data inválida!";
                    return;
                }
                aluno.AlteradoEm = DateTime.Now;
                SqlCommand command = new SqlCommand("UPDATE Alunos SET Nome = @Nome, Classe = @Classe, DataNascimento = @DataNascimento, AlteradoEm = @AlteradoEm WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", aluno.Id);
                command.Parameters.AddWithValue("@Nome", aluno.Nome);
                command.Parameters.AddWithValue("@Classe", aluno.Classe);
                command.Parameters.AddWithValue("@DataNascimento", aluno.DataNascimento);
                command.Parameters.AddWithValue("@AlteradoEm", aluno.AlteradoEm);
                command.ExecuteNonQuery();
            }

        }
        public void LimparCadastro()
        {
            TextBoxNomeAluno.Text = string.Empty;
            TextBoxClasseAluno.Text = string.Empty;
            MaskedTextNascimentoAluno.Text = string.Empty;
            TextRodape.Text = string.Empty;
            TextRodape.Text = "";
            DataGridPresenca.ItemsSource = null;
            _alunoCadastrado = null;
        }
        private void ButtonLimparCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
            LimparCadastro();
        }
        private void ButtonAdicionarFolha_Click(object sender, RoutedEventArgs e)
        {
            FolhaPresencaUI folhaPresencaUI = new FolhaPresencaUI(_alunoCadastrado);
            folhaPresencaUI.Closed += (sender, e) =>
            {
                ListarPresenca();
            };

            folhaPresencaUI.Show();
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
        private void ButtonApagarAluno_Click(object sender, RoutedEventArgs e)
        {
            //PRECISO AJUSTAR O NOME DESSE BOTÃO, ELE NÃO APAGA MAIS O ALUNO, APAGA A FOLHA DE PRESENÇA DENTRO DO CADASTRO DO ALUNO
            TextRodape.Text = "";
            var folhaSelecionada = DataGridPresenca.SelectedItem as FolhaPresenca;
            if(folhaSelecionada == null) { TextRodape.Text = "Selecione uma folha para apagar!"; }

            var result = MessageBox.Show("Deseja realmente apagar a folha de presença?", "Confirmação", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes) 
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM FolhasPresenca WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", folhaSelecionada.Id);
                    command.ExecuteNonQuery();
                    //var folhaExistente = ctx.FolhasPresenca.FirstOrDefault(f => f.Id == folhaSelecionada.Id);
                    //var aluno = ctx.Alunos.FirstOrDefault(a => a.Id == folhaExistente.Aluno_Id);
                    //folhaExistente.Aluno = aluno;
                    //folhaExistente.Aluno_Id = aluno.Id;
                    //ctx.FolhasPresenca.Remove(folhaExistente);
                    //ctx.SaveChanges();
                    ListarPresenca();
                }
            }
        }
        private void ListarPresenca()
        {
            if (_alunoCadastrado != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT Data, Aulas, PresencaNaAula, PossuiAtestadoFalta FROM FolhaPresenca WHERE Aluno_Id = @Aluno_Id ORDER BY Data", connection);
                    command.Parameters.AddWithValue("@Aluno_Id", _alunoCadastrado.Id);
                    SqlDataReader reader = command.ExecuteReader();
                    List<FolhaPresenca> folhas = new List<FolhaPresenca>();
                    while (reader.Read())
                    {
                        FolhaPresenca folha = new FolhaPresenca
                        {
                            Data = reader.GetDateTime(0),
                            Aulas = reader.GetInt32(1),
                            PresencaNaAula = reader.GetDecimal(2),
                            PossuiAtestadoFalta = reader.GetBoolean(3)
                        };
                        folhas.Add(folha);
                    }
                    DataGridPresenca.ItemsSource = folhas;
                }
            }
        }
        private void DataGridPresencaInicializar(object sender, EventArgs e)
        {
            ListarPresenca();
        }
        private void AbriFolha(object sender, MouseButtonEventArgs e)
        {
            var folhaSelecionada = DataGridPresenca.SelectedItem as FolhaPresenca;
            if (folhaSelecionada != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand commandBuscaFolha = new SqlCommand("SELEC Data, Aulas, PresencaNaAula, PossuiAtestadoFalta FROM FolhasPresenca WHERE Id = @Id", connection);
                    commandBuscaFolha.Parameters.AddWithValue("@Id", folhaSelecionada.Id);
                    SqlDataReader reader = commandBuscaFolha.ExecuteReader();
                    FolhaPresenca folhaExiste = null;
                    while (reader.Read())
                    {
                         folhaExiste = new FolhaPresenca
                        {
                            Data = reader.GetDateTime(0),
                            Aulas = reader.GetInt32(1),
                            PresencaNaAula = reader.GetDecimal(2),
                            PossuiAtestadoFalta = reader.GetBoolean(3),
                        };
                    }
                    if (folhaExiste != null)
                    {
                        SqlCommand commandBusca = new SqlCommand("SELECT * FROM Alunos WHERE ID = @ID", connection);
                        commandBusca.Parameters.AddWithValue("@ID", _alunoCadastrado);
                        SqlDataReader readerAluno = commandBusca.ExecuteReader();
                        Aluno aluno = null;
                        while (readerAluno.Read())
                        {
                            aluno = new Aluno
                            {
                                Id = reader.GetGuid(0),
                                Nome = reader.GetString(1),
                                Classe = reader.GetString(2),
                                DataNascimento = reader.GetDateTime(3),
                            };
                        }
                        if (aluno != null)
                        {
                            folhaExiste.Aluno = aluno;
                            FolhaPresencaUI folhaPresenca = new FolhaPresencaUI(folhaExiste, aluno);
                            folhaPresenca.Closed += (sender, e) =>
                            {
                                ListarPresenca();
                            };
                            folhaPresenca.Show();
                        }
                        else
                        {
                            MessageBox.Show("Folha de presença não encontrada.");
                        }

                    }
                }
            }
        }
    }
}
