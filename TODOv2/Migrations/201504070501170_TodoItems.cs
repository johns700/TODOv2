namespace TODOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TodoItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TodoItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Task = c.String(),
                        Complete = c.Boolean(nullable: false),
                        UserID = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TodoItems", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TodoItems", new[] { "ApplicationUser_Id" });
            DropTable("dbo.TodoItems");
        }
    }
}
