namespace Proyeto.GestionRepuestos.MVC.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AjusteBD : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Repuesto",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Nombre = c.String(nullable: false, maxLength: 100),
                    Descripcion = c.String(nullable: false, maxLength: 250),
                    Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CantidadDisponible = c.Int(nullable: false, defaultValue: 0),
                    FechaIngreso = c.DateTime(nullable: false, defaultValueSql: "'1900-01-01T00:00:00.000'"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.SolicitudRepuesto",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    RepuestoId = c.Int(nullable: false),
                    UsuarioSolicitadorId = c.String(nullable: false, maxLength: 128),
                    CantidadSolicitada = c.Int(nullable: false),
                    FechaSolicitud = c.DateTime(nullable: false),
                    Estado = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Repuesto", t => t.RepuestoId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioSolicitadorId)
                .Index(t => t.RepuestoId)
                .Index(t => t.UsuarioSolicitadorId);

            CreateTable(
                "dbo.EntregaRepuestoes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CantidadEntregada = c.Int(nullable: false),
                    FechaEntrega = c.DateTime(nullable: false),
                    SolicitudRepuestoId = c.Int(nullable: false, defaultValue: 0),
                    UsuarioEntregadorId = c.String(nullable: false, maxLength: 128, defaultValue: ""),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudRepuesto", t => t.SolicitudRepuestoId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioEntregadorId)
                .Index(t => t.SolicitudRepuestoId)
                .Index(t => t.UsuarioEntregadorId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.EntregaRepuestoes", "UsuarioEntregadorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EntregaRepuestoes", "SolicitudRepuestoId", "dbo.SolicitudRepuesto");
            DropForeignKey("dbo.SolicitudRepuesto", "UsuarioSolicitadorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SolicitudRepuesto", "RepuestoId", "dbo.Repuesto");
            DropIndex("dbo.EntregaRepuestoes", new[] { "UsuarioEntregadorId" });
            DropIndex("dbo.EntregaRepuestoes", new[] { "SolicitudRepuestoId" });
            DropIndex("dbo.SolicitudRepuesto", new[] { "UsuarioSolicitadorId" });
            DropIndex("dbo.SolicitudRepuesto", new[] { "RepuestoId" });
            DropTable("dbo.EntregaRepuestoes");
            DropTable("dbo.SolicitudRepuesto");
            DropTable("dbo.Repuesto");
        }
    }
}
