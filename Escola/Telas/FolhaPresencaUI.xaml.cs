using System;
using System.Collections.Generic;
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
    /// Lógica interna para FolhaPresencaUI.xaml
    /// </summary>
    public partial class FolhaPresencaUI : Window
    {
        public FolhaPresencaUI()
        {
            InitializeComponent();
        }
        public FolhaPresencaUI(Aluno aluno)
        {
            InitializeComponent();
            TextBoxAluno.Text = aluno.Nome.ToString();
        }

        private void ButtonSalvarFolha_Click(object sender, RoutedEventArgs e)
        {
            FolhaPresenca novaFolhaPresenca = new FolhaPresenca();
            novaFolhaPresenca.Id = Guid.NewGuid();
            using(BancoContext ctx = new BancoContext())
            {
                Aluno aluno = ctx.Alunos.Find(TextBoxAluno.Text);
                novaFolhaPresenca.Aluno = aluno;
            }
            novaFolhaPresenca.Data = DateTime.Parse(TextBoxDataFolha.Text);
            novaFolhaPresenca.Aulas = int.Parse(TextBoxAulas.Text);
            novaFolhaPresenca.PresencaNaAula = decimal.Parse(TextBoxPercentualPresenca.Text);
            novaFolhaPresenca.PossuiAtestadoFalta = CheckBoxAtestado.IsChecked.Value;
        }

        private void ChecarAluno(object sender, DependencyPropertyChangedEventArgs e)
        {
            using (BancoContext ctx = new BancoContext())
            {
                Aluno aluno = ctx.Alunos.Find(TextBoxAluno.Text);
                if (aluno == null)
                {
                    TextRodape.Text = "Aluno não cadastrado!";
                }
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
            TextBoxAluno.Text = "";
            TextBoxDataFolha.Text = "";
            TextBoxAulas.Text = "";
            TextBoxPercentualPresenca.Text = "";
            CheckBoxAtestado.IsChecked = false;
        }
    }
}
