namespace TODOv2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserIDFromTodo : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TodoItems", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TodoItems", "UserID", c => c.Int(nullable: false));
        }
    }
}
