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

namespace Escola
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            if(this.WindowState == WindowState.Maximized)
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
            using (BancoContext ctx = new BancoContext())
            {
                DataGridAlunos.ItemsSource = ctx.Alunos.ToList();
            }
        }
        private void ButtonBuscarAluno_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxBusca.Text.Trim().Count() > 0)
            {
                try
                {
                    using (BancoContext ctx = new BancoContext())
                    {
                        var busca = ctx.Alunos.Where(a => a.Nome.Contains(TextBoxBusca.Text)).ToList();
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
                    using (BancoContext ctx = new BancoContext())
                    {
                        ctx.Alunos.Attach(alunoSelecionado);
                        ctx.Alunos.Remove(alunoSelecionado);
                        ctx.SaveChanges();
                        ListarAlunos();
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