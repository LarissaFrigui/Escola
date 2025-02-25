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
            TextBoxNascimentoAluno.Text = aluno.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty;
        }
        private void ButtonSalvarCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
            var dadosValidos = true;
            try
            {
                if (string.IsNullOrWhiteSpace(TextBoxNomeAluno.Text))
                {
                    TextBoxNomeAluno.BorderBrush = Brushes.Red;
                    TextRodape.Text = "Nome inválido!";
                    dadosValidos = false;
                }
                if (!string.IsNullOrWhiteSpace(TextBoxNascimentoAluno.Text) && !DateTime.TryParseExact(TextBoxNascimentoAluno.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    TextBoxNascimentoAluno.BorderBrush = Brushes.Red;
                    TextRodape.Text = "Data inválida!";
                    dadosValidos = false;
                }
                if (!dadosValidos)
                {
                    return;
                }
                else
                {
                    TextBoxNomeAluno.BorderBrush = Brushes.Gray;
                    TextBoxNascimentoAluno.BorderBrush = Brushes.Gray;
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
                novoAluno.DataNascimento = string.IsNullOrWhiteSpace(TextBoxNascimentoAluno.Text) ? (DateTime?)null : DateTime.ParseExact(TextBoxNascimentoAluno.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                novoAluno.AlteradoEm = DateTime.Now;

                SqlCommand command = new SqlCommand("INSERT INTO Alunos (Id, Nome, Classe, DataNascimento, AlteradoEm) VALUES (@Id, @Nome, @Classe, @DataNascimento, @AlteradoEm)", connection);
                command.Parameters.AddWithValue("@Id", novoAluno.Id);
                command.Parameters.AddWithValue("@Nome", novoAluno.Nome);
                command.Parameters.AddWithValue("@Classe", novoAluno.Classe);
                if (novoAluno.DataNascimento.HasValue)
                {
                    command.Parameters.AddWithValue("@DataNascimento", novoAluno.DataNascimento.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@DataNascimento", DBNull.Value);
                }
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
                aluno.DataNascimento = DateTime.ParseExact(TextBoxNascimentoAluno.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
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
        private void ButtonLimparCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
            TextBoxNomeAluno.Text = string.Empty;
            TextBoxClasseAluno.Text = string.Empty;
            TextBoxNascimentoAluno.Text = string.Empty;
            TextRodape.Text = string.Empty;
            TextRodape.Text = "";
            DataGridPresenca.ItemsSource = null;
            _alunoCadastrado = null;
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
        private void ButtonApagarFolhaPresenca_Click(object sender, RoutedEventArgs e)
        {
            TextRodape.Text = "";
            var folhaSelecionada = DataGridPresenca.SelectedItem as FolhaPresenca;
            if (folhaSelecionada != null)
            {

                var result = MessageBox.Show("Deseja realmente apagar a folha de presença?", "Confirmação", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand("DELETE FROM FolhaPresenca WHERE Id = @Id", connection);
                            command.Parameters.AddWithValue("@Id", folhaSelecionada.Id);
                            command.ExecuteNonQuery();
                        }
                        ListarPresenca();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao apagar folha de presença: " + ex.Message);
                    }
                }
            }
            else { TextRodape.Text = "Selecione uma folha para apagar!"; }
        }
        private void ListarPresenca()
        {
            if (_alunoCadastrado != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT id, Data, Aulas, PresencaNaAula, PossuiAtestadoFalta FROM FolhaPresenca WHERE Aluno_Id = @Aluno_Id ORDER BY Data", connection);
                    command.Parameters.AddWithValue("@Aluno_Id", _alunoCadastrado.Id);
                    SqlDataReader reader = command.ExecuteReader();
                    List<FolhaPresenca> folhas = new List<FolhaPresenca>();
                    while (reader.Read())
                    {
                        FolhaPresenca folha = new FolhaPresenca
                        {
                            Id = reader.GetGuid(0),
                            Data = reader.GetDateTime(1),
                            Aulas = reader.GetInt32(2),
                            PresencaNaAula = reader.GetDecimal(3),
                            PossuiAtestadoFalta = reader.GetBoolean(4)
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
            FolhaPresencaUI folhaPresencaUI = new FolhaPresencaUI(folhaSelecionada, _alunoCadastrado);
            folhaPresencaUI.Closed += (sender, e) =>
            {
                ListarPresenca();
            };
            folhaPresencaUI.Show();
        }

        private void Mascara_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }
            var textBox = sender as TextBox;
            string textoAtual = textBox.Text;

            if(textBox.Text.Length == 2 || textoAtual.Length == 5)
            {
                textBox.Text += "/";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void Mascara_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!DateTime.TryParseExact(textBox.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                textBox.BorderBrush = Brushes.Red;
                TextRodape.Text = "Data inválida!";
            }
            else
            {
                textBox.BorderBrush = Brushes.Gray;
                TextRodape.Text = "";
            }
        }

        private void Mascara_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length > 10)
            {
                textBox.Text = textBox.Text.Substring(0, 10);
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}
