﻿namespace BubbleStart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disableexpense : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCategoryClasses", "Disabled", c => c.Boolean(nullable: false, storeType: "bit"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCategoryClasses", "Disabled");
        }
    }
}
