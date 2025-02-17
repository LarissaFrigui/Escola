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
    /// Lógica interna para AlunoUI.xaml
    /// </summary>
    public partial class AlunoUI : Window
    {
        public AlunoUI()
        {
            InitializeComponent();
        }
        public AlunoUI(Aluno aluno)
        {
            InitializeComponent();
            TextBoxNomeAluno.Text = aluno.Nome;
            TextBoxClasseAluno.Text = aluno.Classe;
            TextBoxNascimentoAluno.Text = aluno.DataNascimento.ToString();
        }

        private void ButtonSalvarCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
                Aluno novoAluno = NovoAluno();
                using (BancoContext ctx = new BancoContext())
                {
                    ctx.Alunos.Add(novoAluno);
                    ctx.SaveChanges();
                }
            //preciso que se tiver preenchido e pq tem aluno cadastrado
            //entao tenho que pegar o id do aluno e atualizar as outras informacoes 
            //else 
            //{
            //    using (BancoContext ctx = new BancoContext())
            //    {
            //        Aluno aluno = ctx.Alunos.FirstOrDefault(a => a.Id == alunoId);
            //    }
            //}
            TextRodape.Text = "Salvo com sucesso!";
        }
        private Aluno NovoAluno() 
        {
            Aluno novoAluno = new Aluno();
            novoAluno.Id = Guid.NewGuid();
            novoAluno.Nome = TextBoxNomeAluno.Text;
            novoAluno.Classe = TextBoxClasseAluno.Text;
            novoAluno.DataNascimento = DateTime.Parse(TextBoxNascimentoAluno.Text);
            novoAluno.AlteradoEm = DateTime.Now;
            return novoAluno;
        }
        public void LimparCadastro()
        {
            TextBoxNomeAluno.Text = "";
            TextBoxClasseAluno.Text = "";
            TextBoxNascimentoAluno.Text = "";
            TextRodape.Text = "";
        }
        private void ButtonLimparCadastroAluno_Click(object sender, RoutedEventArgs e)
        {
            LimparCadastro();
        }

        private void ButtonAdicionarFolha_Click(object sender, RoutedEventArgs e)
        {
            Aluno aluno = NovoAluno();
            FolhaPresencaUI folhaPresencaUI = new FolhaPresencaUI(aluno);
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
            using(BancoContext ctx = new BancoContext())
            {
                Aluno aluno = ctx.Alunos.FirstOrDefault(a => a.Nome == TextBoxNomeAluno.Text);
                ctx.Alunos.Remove(aluno);
                ctx.SaveChanges();
            }
        }
    }
}
