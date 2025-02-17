using System.Data.Entity;

namespace Escola.Entidade
{
    public class FolhaPresenca
    {
        public Guid Id { get; set; }
        public Aluno Aluno { get; set; }
        public DateTime? Data { get; set; }
        public int Aulas { get; set; }
        public bool PossuiAtestadoFalta { get; set; }
        public decimal PresencaNaAula { get; set; }

        public FolhaPresenca()
        {
            Id = Guid.Empty;
            Aluno = new Aluno();
            Data = null;
            Aulas = 0;
            PossuiAtestadoFalta = false;
            PresencaNaAula = 0;
        }
    }
}
