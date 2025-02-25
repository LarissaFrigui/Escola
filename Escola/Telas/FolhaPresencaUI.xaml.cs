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
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Escola.Telas
{
    /// <summary>
    /// Lógica interna para FolhaPresencaUI.xaml
    /// </summary>

    // Quando eu clico fora da celulas da linha da folha de presença ele da erro, verificar como resolver isso

    public partial class FolhaPresencaUI : Window
    {
        FolhaPresenca _folhaVerifica = null;
        Guid _idAluno;

        public FolhaPresencaUI(Aluno aluno)
        {
            InitializeComponent();
            TextBoxAluno.Text = aluno.Nome.ToString();
            _idAluno = aluno.Id;            
        }
        public FolhaPresencaUI(FolhaPresenca folhaPresenca, Aluno aluno)
        {
            _folhaVerifica = folhaPresenca;
            _idAluno = aluno.Id;
            InitializeComponent();
            TextBoxAluno.Text = aluno.Nome;
            TextBoxDataAula.Text = folhaPresenca.Data?.ToString("dd/MM/yyyy") ?? string.Empty;
            TextBoxAulas.Text = folhaPresenca.Aulas.ToString();
            TextBoxPercentualPresenca.Text = folhaPresenca.PresencaNaAula.ToString();
            CheckBoxAtestado.IsChecked = folhaPresenca.PossuiAtestadoFalta;
        }
        private void ButtonSalvarFolha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_folhaVerifica == null)
                {
                    NovaFolhaDePresenca();
                }
                else
                {
                    AtualizaFolhaPresenca(_folhaVerifica);
                }

                TextRodape.Text = "Salvo com sucesso!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TextRodape.Text = "Erro inesperado ao salvar os dados!";
            }
        }
        private void NovaFolhaDePresenca()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Aluno aluno = BuscaAluno();
                if (aluno == null)
                {
                    MessageBox.Show("Aluno não cadastrado");
                    return;
                }
                FolhaPresenca novaFolha = new FolhaPresenca();
                novaFolha.Id = Guid.NewGuid();

                if (DateTime.TryParseExact(TextBoxDataAula.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Data))
                {
                    novaFolha.Data = Data;
                }
                else
                {
                    TextRodape.Text = "Data inválida!";
                    return;
                }
                novaFolha.Aulas = int.Parse(TextBoxAulas.Text);
                novaFolha.Aluno_Id = aluno.Id;
                novaFolha.PresencaNaAula = decimal.Parse(TextBoxPercentualPresenca.Text);
                novaFolha.PossuiAtestadoFalta = CheckBoxAtestado.IsChecked.Value;

                SqlCommand commandInsert = new SqlCommand("INSERT INTO FolhaPresenca (ID, Aluno_ID, Data, Aulas, PossuiAtestadoFalta, PresencaNaAula) VALUES (@ID, @Aluno_ID, @Data, @Aulas, @PossuiAtestadoFalta, @PresencaNaAula)", connection);
                commandInsert.Parameters.AddWithValue("@ID", novaFolha.Id);
                commandInsert.Parameters.AddWithValue("@Aluno_ID", novaFolha.Aluno_Id);
                commandInsert.Parameters.AddWithValue("@Data", novaFolha.Data);
                commandInsert.Parameters.AddWithValue("@Aulas", novaFolha.Aulas);
                commandInsert.Parameters.AddWithValue("@PossuiAtestadoFalta", novaFolha.PossuiAtestadoFalta);
                commandInsert.Parameters.AddWithValue("@PresencaNaAula", novaFolha.PresencaNaAula);
                commandInsert.ExecuteNonQuery();
                _folhaVerifica = novaFolha;
            }
        }
        private Aluno BuscaAluno()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand commandBusca = new SqlCommand("SELECT * FROM Alunos WHERE ID = @ID", connection);
                commandBusca.Parameters.AddWithValue("@ID", _idAluno);
                SqlDataReader reader = commandBusca.ExecuteReader();
                Aluno aluno = null;
                while (reader.Read())
                {
                    aluno = new Aluno
                    {
                        Id = reader.GetGuid(0),
                        Nome = reader.GetString(1),
                        Classe = reader.GetString(2),
                        DataNascimento = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                    };
                }
                return aluno;
            }
        }
        private void AtualizaFolhaPresenca(FolhaPresenca folha)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BDEscolaADO"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                folha.Aulas = int.Parse(TextBoxAulas.Text);
                folha.Data = DateTime.ParseExact(TextBoxDataAula.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                folha.PresencaNaAula = decimal.Parse(TextBoxPercentualPresenca.Text);
                folha.PossuiAtestadoFalta = CheckBoxAtestado.IsChecked.Value;

                SqlCommand commandUpdate = new SqlCommand("UPDATE FolhaPresenca SET Data = @Data, Aulas = @Aulas, PresencaNaAula = @PresencaNaAula, PossuiAtestadoFalta = @PossuiAtestadoFalta WHERE Id = @Id", connection);
                commandUpdate.Parameters.AddWithValue("@Id", folha.Id);
                commandUpdate.Parameters.AddWithValue("@Data", folha.Data);
                commandUpdate.Parameters.AddWithValue("@Aulas", folha.Aulas);
                commandUpdate.Parameters.AddWithValue("@PresencaNaAula", folha.PresencaNaAula);
                commandUpdate.Parameters.AddWithValue("@PossuiAtestadoFalta", folha.PossuiAtestadoFalta);
                commandUpdate.ExecuteNonQuery();
            }
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

        private void ButtonLimparFolha_Click(object sender, RoutedEventArgs e)
        {
            TextBoxDataAula.Text = "";
            TextBoxAulas.Text = "";
            TextBoxPercentualPresenca.Text = "";
            CheckBoxAtestado.IsChecked = false;
            TextBoxDataAula.BorderBrush = new SolidColorBrush(Colors.Black); 
            TextBoxAulas.BorderBrush = new SolidColorBrush(Colors.Black);
            TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Black);
            TextRodape.Text = "";
        }
        private void MascaraData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }
            var textBox = sender as TextBox;
            string textoAtual = textBox.Text;

            if (textBox.Text.Length == 2 || textoAtual.Length == 5)
            {
                textBox.Text += "/";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void MascaraData_LostFocus(object sender, RoutedEventArgs e)
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

        private void MascaraData_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length > 10)
            {
                textBox.Text = textBox.Text.Substring(0, 10);
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void MascaraAulas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }
        }

        private void MascaraPresenca_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(char.IsDigit(e.Text,e.Text.Length - 1) || e.Text == ",")
            {
                e.Handled = false;
                return;
            }
        }

        private void MascaraPresenca_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(decimal.TryParse(textBox.Text, out decimal valor) && valor > 100)
            {
                textBox.BorderBrush = Brushes.Red;
                TextRodape.Text = "Percentual de presença não pode ser maior que 100%";
            }
            else
            {
                textBox.BorderBrush = Brushes.Gray;
                TextRodape.Text = "";
            }
        }
    }
}
