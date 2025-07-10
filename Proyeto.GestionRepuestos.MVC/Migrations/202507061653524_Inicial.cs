namespace Proyeto.GestionRepuestos.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EntregaRepuestoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RepuestoId = c.Int(nullable: false),
                        Mecanico = c.String(nullable: false, maxLength: 100),
                        CantidadEntregada = c.Int(nullable: false),
                        FechaEntrega = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Repuestoes", t => t.RepuestoId, cascadeDelete: true)
                .Index(t => t.RepuestoId);
            
            CreateTable(
                "dbo.Repuestoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Descripcion = c.String(nullable: false, maxLength: 250),
                        Cantidad = c.Int(nullable: false),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EntregaRepuestoes", "RepuestoId", "dbo.Repuestoes");
            DropIndex("dbo.EntregaRepuestoes", new[] { "RepuestoId" });
            DropTable("dbo.Repuestoes");
            DropTable("dbo.EntregaRepuestoes");
        }
    }
}
