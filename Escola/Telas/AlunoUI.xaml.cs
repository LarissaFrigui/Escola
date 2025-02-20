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

namespace Escola.Telas
{
    /// <summary>
    /// Lógica interna para AlunoUI.xaml
    /// </summary>
    public partial class AlunoUI : Window
    {
        Aluno _alunoCadastrado = null;

        //PRECISO AJUSTAR O DELETE DE FOLHA DE PRESENCA

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
            using (BancoContext ctx = new BancoContext())
            {
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
                ctx.Alunos.Add(novoAluno);
                ctx.SaveChanges();
                _alunoCadastrado = novoAluno;
            }
        }
        private void Atualiza(Aluno aluno)
        {
            using (BancoContext ctx = new BancoContext())
            {
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
                ctx.Alunos.Attach(aluno);
                ctx.Entry(aluno).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
            }

        }
        public void LimparCadastro()
        {
            TextBoxNomeAluno.Text = string.Empty;
            TextBoxClasseAluno.Text = string.Empty;
            MaskedTextNascimentoAluno.Text = string.Empty;
            TextRodape.Text = string.Empty;
            TextRodape.Text = "";
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
            TextRodape.Text = "";
            var folhaSelecionada = DataGridPresenca.SelectedItem as FolhaPresenca;
            if (folhaSelecionada != null)
            {
                using (BancoContext ctx = new BancoContext())
                {
                    var alunoExiste = ctx.Alunos.FirstOrDefault(a => a.Id == folhaSelecionada.Aluno_Id);
                    if (alunoExiste == null) { TextRodape.Text = "Aluno não encontrado!"; return; }

                    var result = MessageBox.Show("Deseja realmente apagar o folha de presença?", "Confirmação", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            ctx.FolhasPresenca.Remove(folhaSelecionada);
                            ctx.SaveChanges();
                            ListarPresenca();
                        }
                        catch (Exception excecao)
                        {
                            Console.WriteLine(excecao);
                            TextRodape.Text = "Erro inesperado! Por favor tente novamente mais tarde.";
                        }
                    }
                }
            }
            else { TextRodape.Text = "Selecione uma folha para apagar!"; }
        }
        private void ListarPresenca()
        {
            if (_alunoCadastrado != null)
            {
                using (BancoContext ctx = new BancoContext())
                {
                    DataGridPresenca.ItemsSource = ctx.FolhasPresenca.Where(a => a.Aluno_Id == _alunoCadastrado.Id).ToList();
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
                using (BancoContext ctx = new BancoContext())
                {
                    var folha = ctx.FolhasPresenca.FirstOrDefault(f => f.Id == folhaSelecionada.Id);
                    if (folha != null)
                    {
                        var aluno = ctx.Alunos.FirstOrDefault(a => a.Id == folha.Aluno_Id);
                        if (aluno != null)
                        {
                            folha.Aluno = aluno;
                            FolhaPresencaUI folhaPresenca = new FolhaPresencaUI(folha, aluno);
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
