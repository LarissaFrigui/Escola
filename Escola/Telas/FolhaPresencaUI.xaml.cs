using System;
using System.Collections.Generic;
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

namespace Escola.Telas
{
    /// <summary>
    /// Lógica interna para FolhaPresencaUI.xaml
    /// </summary>
    public partial class FolhaPresencaUI : Window
    {
        //testar se a parte do checar ainda esta funcionando 
        //verificar se o inicializar sem parametros esta precisa
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

                bool dataValida = DateTime.TryParse(MaskedTextData.Text, out DateTime data);
                if (!dataValida)
                {
                    MaskedTextData.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
                }
                bool aulasValidas = int.TryParse(TextBoxAulas.Text, out int aulas);
                if (!aulasValidas)
                {
                    TextBoxAulas.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
                }
                //colocar um maximo de 100%
                bool percentualValido = decimal.TryParse(TextBoxPercentualPresenca.Text, out decimal percentual);
                if (!percentualValido)
                {
                    TextBoxPercentualPresenca.BorderBrush = new SolidColorBrush(Colors.Red);
                    dadosInvalidos = true;
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
            using (BancoContext ctx = new BancoContext())
            {
                var aluno = ctx.Alunos.FirstOrDefault(a => a.Id == _idAluno);
                if (aluno == null)
                {
                    MessageBox.Show("Aluno não cadastrado");
                    return;
                }

                FolhaPresenca novaFolha = new FolhaPresenca();

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

                ctx.FolhasPresenca.Add(novaFolha);
                ctx.SaveChanges();
            }
        }
        private void AtualizaFolhaPresenca(FolhaPresenca folha)
        {
            using (BancoContext ctx = new BancoContext())
            {
                var folhaExistente = ctx.FolhasPresenca.Include(f => f.Aluno).FirstOrDefault(f => f.Id == folha.Id);
                if (folhaExistente == null)
                {
                    TextRodape.Text = "Folha de presença não encontrada!";
                    return;
                }
                if (DateTime.TryParseExact(MaskedTextData.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Data))
                {
                    folhaExistente.Data = Data;
                }
                else
                {
                    TextRodape.Text = "Data inválida!";
                    return;
                }
                folhaExistente.Aulas = int.Parse(TextBoxAulas.Text);
                folhaExistente.PresencaNaAula = decimal.Parse(TextBoxPercentualPresenca.Text);
                folhaExistente.PossuiAtestadoFalta = CheckBoxAtestado.IsChecked.Value;

                var aluno = ctx.Alunos.FirstOrDefault(a => a.Id == _idAluno);
                if (aluno == null)
                {
                    TextRodape.Text = "Aluno não cadastrado";
                    return;
                }
                folhaExistente.Aluno = aluno;
                folhaExistente.Aluno_Id = aluno.Id;

                //ctx.FolhasPresenca.Attach(folhaExistente);
                ctx.Entry(folhaExistente).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
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

        private void ChecarAluno(object sender, RoutedEventArgs e)
        {
            bool verificacao = ChecarAlunoValidacao();
        }
        private bool ChecarAlunoValidacao()
        {
            using (BancoContext ctx = new BancoContext())
            {
                Aluno aluno = ctx.Alunos.FirstOrDefault(a => a.Nome == TextBoxAluno.Text);
                if (aluno == null)
                {
                    TextRodape.Text = "Aluno não cadastrado!";
                    TextBoxAluno.Foreground = new SolidColorBrush(Colors.Red);
                    return false;
                }
                else
                {
                    TextRodape.Text = "";
                    TextBoxAluno.Foreground = new SolidColorBrush(Colors.Black);
                    return true;
                }
            }
        }
    }
}
