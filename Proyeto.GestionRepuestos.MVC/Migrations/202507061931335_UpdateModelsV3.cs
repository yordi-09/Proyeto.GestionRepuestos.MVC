namespace Proyeto.GestionRepuestos.MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelsV3 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Repuestoes", newName: "Repuesto");
            DropForeignKey("dbo.EntregaRepuestoes", "RepuestoId", "dbo.Repuestoes");
            DropIndex("dbo.EntregaRepuestoes", new[] { "RepuestoId" });
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
            
            AddColumn("dbo.EntregaRepuestoes", "SolicitudRepuestoId", c => c.Int(nullable: false));
            AddColumn("dbo.EntregaRepuestoes", "UsuarioEntregadorId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Repuesto", "CantidadDisponible", c => c.Int(nullable: false));
            AddColumn("dbo.Repuesto", "FechaIngreso", c => c.DateTime(nullable: false));
            CreateIndex("dbo.EntregaRepuestoes", "SolicitudRepuestoId");
            CreateIndex("dbo.EntregaRepuestoes", "UsuarioEntregadorId");
            AddForeignKey("dbo.EntregaRepuestoes", "SolicitudRepuestoId", "dbo.SolicitudRepuesto", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EntregaRepuestoes", "UsuarioEntregadorId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.EntregaRepuestoes", "RepuestoId");
            DropColumn("dbo.EntregaRepuestoes", "Mecanico");
            DropColumn("dbo.Repuesto", "Cantidad");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Repuesto", "Cantidad", c => c.Int(nullable: false));
            AddColumn("dbo.EntregaRepuestoes", "Mecanico", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.EntregaRepuestoes", "RepuestoId", c => c.Int(nullable: false));
            DropForeignKey("dbo.EntregaRepuestoes", "UsuarioEntregadorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EntregaRepuestoes", "SolicitudRepuestoId", "dbo.SolicitudRepuesto");
            DropForeignKey("dbo.SolicitudRepuesto", "UsuarioSolicitadorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SolicitudRepuesto", "RepuestoId", "dbo.Repuesto");
            DropIndex("dbo.SolicitudRepuesto", new[] { "UsuarioSolicitadorId" });
            DropIndex("dbo.SolicitudRepuesto", new[] { "RepuestoId" });
            DropIndex("dbo.EntregaRepuestoes", new[] { "UsuarioEntregadorId" });
            DropIndex("dbo.EntregaRepuestoes", new[] { "SolicitudRepuestoId" });
            DropColumn("dbo.Repuesto", "FechaIngreso");
            DropColumn("dbo.Repuesto", "CantidadDisponible");
            DropColumn("dbo.EntregaRepuestoes", "UsuarioEntregadorId");
            DropColumn("dbo.EntregaRepuestoes", "SolicitudRepuestoId");
            DropTable("dbo.SolicitudRepuesto");
            CreateIndex("dbo.EntregaRepuestoes", "RepuestoId");
            AddForeignKey("dbo.EntregaRepuestoes", "RepuestoId", "dbo.Repuestoes", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.Repuesto", newName: "Repuestoes");
        }
    }
}
