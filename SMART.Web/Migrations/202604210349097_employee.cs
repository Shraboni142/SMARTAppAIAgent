namespace SMART.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employee : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "hrm.EmployeeComplains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        ReviewStatus = c.String(),
                        OffenceType = c.String(),
                        OffenceDetails = c.String(),
                        ComplainActionType = c.String(),
                        ComplainActionDetails = c.String(),
                        DateOfNotice = c.DateTime(nullable: false),
                        EarlyWithdrawalDate = c.DateTime(),
                        AttachmentFileName = c.String(),
                        AttachmentFilePath = c.String(),
                        CreatedBy = c.Int(),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedOn = c.DateTime(),
                        DeletedBy = c.Int(),
                        DeletedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("hrm.Employees", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "hrm.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedBy = c.Int(),
                        CreatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedOn = c.DateTime(),
                        DeletedBy = c.Int(),
                        DeletedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("hrm.EmployeeComplains", "EmployeeId", "hrm.Employees");
            DropIndex("hrm.EmployeeComplains", new[] { "EmployeeId" });
            DropTable("hrm.Employees");
            DropTable("hrm.EmployeeComplains");
        }
    }
}
