namespace Escola.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ajuste : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FolhaPresencas", "Aluno_Id", "dbo.Alunoes");
            DropIndex("dbo.FolhaPresencas", new[] { "Aluno_Id" });
            AlterColumn("dbo.FolhaPresencas", "Aluno_Id", c => c.Guid(nullable: false));
            CreateIndex("dbo.FolhaPresencas", "Aluno_Id");
            AddForeignKey("dbo.FolhaPresencas", "Aluno_Id", "dbo.Alunoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FolhaPresencas", "Aluno_Id", "dbo.Alunoes");
            DropIndex("dbo.FolhaPresencas", new[] { "Aluno_Id" });
            AlterColumn("dbo.FolhaPresencas", "Aluno_Id", c => c.Guid());
            CreateIndex("dbo.FolhaPresencas", "Aluno_Id");
            AddForeignKey("dbo.FolhaPresencas", "Aluno_Id", "dbo.Alunoes", "Id");
        }
    }
}
