using Escola.Entidade;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escola
{
    public class BancoContext : DbContext
    {
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<FolhaPresenca> FolhasPresenca { get; set; }
        public BancoContext() : base("name=BDEscolaCRUD")
        {
        }

    }
}