namespace Escola.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alunoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(),
                        Classe = c.String(),
                        DataNascimento = c.DateTime(),
                        AlteradoEm = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FolhaPresencas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Data = c.DateTime(),
                        Aulas = c.Int(nullable: false),
                        PossuiAtestadoFalta = c.Boolean(nullable: false),
                        PresencaNaAula = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Aluno_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Alunoes", t => t.Aluno_Id)
                .Index(t => t.Aluno_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FolhaPresencas", "Aluno_Id", "dbo.Alunoes");
            DropIndex("dbo.FolhaPresencas", new[] { "Aluno_Id" });
            DropTable("dbo.FolhaPresencas");
            DropTable("dbo.Alunoes");
        }
    }
}
