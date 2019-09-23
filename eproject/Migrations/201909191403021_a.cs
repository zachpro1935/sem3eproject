namespace eproject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contests",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30),
                        content = c.String(nullable: false, maxLength: 3000),
                        createAt = c.DateTime(nullable: false),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        winner = c.Guid(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30),
                        category = c.String(nullable: false),
                        content = c.String(nullable: false),
                        type = c.String(nullable: false),
                        createAt = c.DateTime(nullable: false),
                        enabled = c.Boolean(nullable: false),
                        image = c.String(),
                        manager = c.Guid(nullable: false),
                        contest_id = c.Guid(),
                        viewCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Contests", t => t.contest_id)
                .ForeignKey("dbo.Users", t => t.manager, cascadeDelete: false)
                .Index(t => t.manager)
                .Index(t => t.contest_id);
            
            CreateTable(
                "dbo.FeedBacks",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        content = c.String(nullable: false, maxLength: 150),
                        createAt = c.DateTime(nullable: false),
                        own = c.Guid(nullable: false),
                        recipe_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Recipes", t => t.recipe_id, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.own, cascadeDelete: true)
                .Index(t => t.own)
                .Index(t => t.recipe_id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30),
                        pass = c.String(nullable: false),
                        email = c.String(nullable: false, maxLength: 30),
                        gender = c.String(nullable: false),
                        phone = c.String(nullable: false),
                        role = c.String(),
                        expireDate = c.DateTime(nullable: false),
                        enabled = c.Boolean(nullable: false),
                        emailVerify = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.email, unique: true);
            
            CreateTable(
                "dbo.Rattings",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        rate = c.Int(nullable: false),
                        own = c.Guid(nullable: false),
                        recipe_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Recipes", t => t.recipe_id, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.own, cascadeDelete: false)
                .Index(t => t.own)
                .Index(t => t.recipe_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rattings", "own", "dbo.Users");
            DropForeignKey("dbo.Rattings", "recipe_id", "dbo.Recipes");
            DropForeignKey("dbo.Recipes", "manager", "dbo.Users");
            DropForeignKey("dbo.FeedBacks", "own", "dbo.Users");
            DropForeignKey("dbo.FeedBacks", "recipe_id", "dbo.Recipes");
            DropForeignKey("dbo.Recipes", "contest_id", "dbo.Contests");
            DropIndex("dbo.Rattings", new[] { "recipe_id" });
            DropIndex("dbo.Rattings", new[] { "own" });
            DropIndex("dbo.Users", new[] { "email" });
            DropIndex("dbo.FeedBacks", new[] { "recipe_id" });
            DropIndex("dbo.FeedBacks", new[] { "own" });
            DropIndex("dbo.Recipes", new[] { "contest_id" });
            DropIndex("dbo.Recipes", new[] { "manager" });
            DropTable("dbo.Rattings");
            DropTable("dbo.Users");
            DropTable("dbo.FeedBacks");
            DropTable("dbo.Recipes");
            DropTable("dbo.Contests");
        }
    }
}
