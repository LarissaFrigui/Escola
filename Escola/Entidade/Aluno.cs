namespace Escola.Entidade
{
    public class Aluno
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Classe { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? AlteradoEm { get; set; }

        public Aluno()
        {
            Id = Guid.Empty;
            Nome =
                Classe = string.Empty;
            AlteradoEm = null;
            DataNascimento = null;
        }
    }
}
