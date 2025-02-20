using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Escola.Entidade
{
    public class FolhaPresenca
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Aluno_Id")]
        public Aluno Aluno { get; set; }

        public Guid Aluno_Id { get; set; } // Chave estrangeira para o Aluno
        public DateTime? Data { get; set; }
        public int Aulas { get; set; }
        public bool PossuiAtestadoFalta { get; set; }
        public decimal PresencaNaAula { get; set; }

        public FolhaPresenca()
        {
            Id = Guid.NewGuid();
            Aluno = new Aluno();
            Data = null;
            Aulas = 0;
            PossuiAtestadoFalta = false;
            PresencaNaAula = 0;
        }

    }
}
