namespace Proyeto.GestionRepuestos.MVC.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class renombrar : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EntregaRepuestoes", newName: "EntregaRepuesto");
        }

        public override void Down()
        {
            RenameTable(name: "dbo.EntregaRepuesto", newName: "EntregaRepuestoes");
        }
    }
}
