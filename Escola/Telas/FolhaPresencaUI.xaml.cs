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
            TextBoxAluno.Text = folhaPresenca.Aluno.Nome;
            MaskedTextData.Text = folhaPresenca.Data?.ToString("dd/MM/yyyy") ?? string.Empty;
            TextBoxAulas.Text = folhaPresenca.Aulas.ToString();
            TextBoxPercentualPresenca.Text = folhaPresenca.PresencaNaAula.ToString();
            CheckBoxAtestado.IsChecked = folhaPresenca.PossuiAtestadoFalta;
        }

        private void ButtonSalvarFolha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool dadosInvalidos= false;
                //Talvez a alteracao de cor de para colocar junto com o lostfocus...
                bool dataValida = DateTime.TryParse(MaskedTextData.Text, out DateTime data);
                if (!dataValida)
                {
                    MaskedTextData.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
                }
                else
                {
                    MaskedTextData.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                bool aulasValidas = int.TryParse(TextBoxAulas.Text, out int aulas);
                if (!aulasValidas)
                {
                    TextBoxAulas.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
                }
                else
                {
                    TextBoxAulas.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                bool percentualValido = decimal.TryParse(TextBoxPercentualPresenca.Text, out decimal percentual);
                if (!percentualValido)
                {
                    TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
                }
                else
                {
                    TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                if (percentual > 100)
                {
                    TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
                    TextRodape.Text = "Percentual de presença não pode ser maior que 100%";
                }
                else
                {
                    TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Black);
                }

                if (dadosInvalidos)
                    return;

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

                if (DateTime.TryParseExact(MaskedTextData.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Data))
                {
                    novaFolha.Data = Data;
                }
                else
                {
                    TextRodape.Text = "Data inválida!";
                    return;
                }
                novaFolha.Aulas = int.Parse(TextBoxAulas.Text);
                novaFolha.Aluno = aluno;
                novaFolha.Aluno_Id = aluno.Id;
                novaFolha.PresencaNaAula = decimal.Parse(TextBoxPercentualPresenca.Text);
                novaFolha.PossuiAtestadoFalta = CheckBoxAtestado.IsChecked.Value;

                SqlCommand commandInsert = new SqlCommand("INSERT INTO FolhasPresenca (Id, Aluno, Aluno_Id, Data, Aulas, PresencaNaAula, PossuiAtestadoFalta) VALUES (@Id, @Aluno, @Aluno_Id, @Data, @Aulas, @PresencaNaAula, @PossuiAtestadoFalta)", connection);
                commandInsert.Parameters.AddWithValue("@Id", novaFolha.Id);
                commandInsert.Parameters.AddWithValue("@Aluno_Id", novaFolha.Aluno_Id);
                commandInsert.Parameters.AddWithValue("@Aluno", novaFolha.Aluno);
                commandInsert.Parameters.AddWithValue("@Data", novaFolha.Data);
                commandInsert.Parameters.AddWithValue("@Aulas", novaFolha.Aulas);
                commandInsert.Parameters.AddWithValue("@PresencaNaAula", novaFolha.PresencaNaAula);
                commandInsert.Parameters.AddWithValue("@PossuiAtestadoFalta", novaFolha.PossuiAtestadoFalta);
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
                        DataNascimento = reader.GetDateTime(3),
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
                Aluno aluno = BuscaAluno();
                if (aluno == null)
                {
                    MessageBox.Show("Aluno não cadastrado");
                    return;
                }
                SqlCommand commandBuscaFolha = new SqlCommand("SELEC Data, Aulas, PresencaNaAula, PossuiAtestadoFalta FROM FolhasPresenca WHERE Id = @Id", connection);
                commandBuscaFolha.Parameters.AddWithValue("@Id", folha.Id);
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
                if (DateTime.TryParseExact(MaskedTextData.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Data))
                {
                    folha.Data = Data;
                }
                else
                {
                    TextRodape.Text = "Data inválida!";
                    return;
                }
                folha.Aulas = int.Parse(TextBoxAulas.Text);
                folha.PresencaNaAula = decimal.Parse(TextBoxPercentualPresenca.Text);
                folha.PossuiAtestadoFalta = CheckBoxAtestado.IsChecked.Value;

                SqlCommand commandUpdate = new SqlCommand("UPDATE FolhasPresenca SET Data = @Data, Aulas = @Aulas, PresencaNaAula = @PresencaNaAula, PossuiAtestadoFalta = @PossuiAtestadoFalta WHERE Id = @Id", connection);
                commandUpdate.Parameters.AddWithValue("@Id", folha.Id);
                commandUpdate.Parameters.AddWithValue("@Data", folha.Data);
                commandUpdate.Parameters.AddWithValue("@Aulas", folha.Aulas);
                commandUpdate.Parameters.AddWithValue("@PresencaNaAula", folha.PresencaNaAula);
                commandUpdate.Parameters.AddWithValue("@PossuiAtestadoFalta", folha.PossuiAtestadoFalta);
                commandUpdate.ExecuteNonQuery();
                TextRodape.Text = "Folha de presença atualizada com sucesso!";
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
            MaskedTextData.Text = "";
            TextBoxAulas.Text = "";
            TextBoxPercentualPresenca.Text = "";
            CheckBoxAtestado.IsChecked = false;
            MaskedTextData.BorderBrush = new SolidColorBrush(Colors.Black); 
            TextBoxAulas.BorderBrush = new SolidColorBrush(Colors.Black);
            TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Black);
            TextRodape.Text = "";
        }
    }
}
