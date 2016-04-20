namespace MicrosoftSyncPoC.Repository.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Guid(nullable: false),
                        CategoryName = c.String(nullable: false, maxLength: 35),
                        Description = c.String(),
                        Picture = c.Binary(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Guid(nullable: false),
                        ProductName = c.String(nullable: false, maxLength: 40),
                        SupplierID = c.Guid(),
                        CategoryID = c.Guid(),
                        QuantityPerUnit = c.String(maxLength: 35),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        UnitsInStock = c.Short(),
                        UnitsOnOrder = c.Short(),
                        ReorderLevel = c.Short(),
                        Discontinued = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.SupplierID)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceID = c.Guid(nullable: false),
                        CustomerID = c.Guid(nullable: false),
                        Salesperson = c.Guid(nullable: false),
                        OrderID = c.Guid(nullable: false),
                        ShipperID = c.Guid(nullable: false),
                        ProductID = c.Guid(nullable: false),
                        ShipAddress = c.String(maxLength: 60),
                        ShipCity = c.String(maxLength: 35),
                        ShipRegion = c.String(maxLength: 35),
                        ShipPostalCode = c.String(maxLength: 35),
                        ShipCountry = c.String(maxLength: 35),
                        CustomerName = c.String(nullable: false, maxLength: 40),
                        Address = c.String(maxLength: 60),
                        City = c.String(maxLength: 15),
                        RegionID = c.Guid(nullable: false),
                        PostalCode = c.String(maxLength: 10),
                        Country = c.String(maxLength: 15),
                        OrderDate = c.DateTime(),
                        RequiredDate = c.DateTime(),
                        ShippedDate = c.DateTime(),
                        ShipperName = c.String(nullable: false, maxLength: 40),
                        ProductName = c.String(nullable: false, maxLength: 40),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Short(nullable: false),
                        Discount = c.Single(nullable: false),
                        ExtendedPrice = c.Decimal(precision: 18, scale: 2),
                        Freight = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.InvoiceID, t.CustomerID, t.Salesperson, t.OrderID, t.ShipperID, t.ProductID })
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.ShipperID, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Region", t => t.RegionID, cascadeDelete: true)
                .ForeignKey("dbo.Shippers", t => t.ShipperID, cascadeDelete: true)
                .Index(t => t.CustomerID)
                .Index(t => t.OrderID)
                .Index(t => t.ShipperID)
                .Index(t => t.ProductID)
                .Index(t => t.RegionID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Guid(nullable: false),
                        CompanyName = c.String(nullable: false, maxLength: 40),
                        ContactName = c.String(maxLength: 35),
                        ContactTitle = c.String(maxLength: 35),
                        Address = c.String(maxLength: 60),
                        City = c.String(maxLength: 35),
                        Region = c.String(maxLength: 35),
                        PostalCode = c.String(maxLength: 35),
                        Country = c.String(maxLength: 35),
                        Phone = c.String(maxLength: 35),
                        Fax = c.String(maxLength: 35),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Guid(nullable: false),
                        CustomerID = c.Guid(),
                        EmployeeID = c.Guid(),
                        OrderDate = c.DateTime(),
                        RequiredDate = c.DateTime(),
                        ShippedDate = c.DateTime(),
                        ShipVia = c.Guid(),
                        Freight = c.Decimal(precision: 18, scale: 2),
                        ShipName = c.String(maxLength: 40),
                        ShipAddress = c.String(maxLength: 60),
                        ShipCity = c.String(maxLength: 35),
                        ShipRegion = c.String(maxLength: 35),
                        ShipPostalCode = c.String(maxLength: 35),
                        ShipCountry = c.String(maxLength: 35),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .ForeignKey("dbo.Shippers", t => t.ShipVia)
                .Index(t => t.CustomerID)
                .Index(t => t.EmployeeID)
                .Index(t => t.ShipVia);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Guid(nullable: false),
                        LastName = c.String(nullable: false, maxLength: 35),
                        FirstName = c.String(nullable: false, maxLength: 35),
                        Title = c.String(maxLength: 35),
                        TitleOfCourtesy = c.String(maxLength: 35),
                        BirthDate = c.DateTime(),
                        HireDate = c.DateTime(),
                        Address = c.String(maxLength: 60),
                        City = c.String(maxLength: 35),
                        Region = c.String(maxLength: 35),
                        PostalCode = c.String(maxLength: 35),
                        Country = c.String(maxLength: 35),
                        HomePhone = c.String(maxLength: 35),
                        Extension = c.String(maxLength: 35),
                        Photo = c.Binary(),
                        Notes = c.String(),
                        ReportsTo = c.Guid(),
                        PhotoPath = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Employees", t => t.ReportsTo)
                .Index(t => t.ReportsTo);
            
            CreateTable(
                "dbo.Territories",
                c => new
                    {
                        TerritoryID = c.Guid(nullable: false),
                        TerritoryDescription = c.String(nullable: false, maxLength: 50, fixedLength: true),
                        RegionID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TerritoryID)
                .ForeignKey("dbo.Region", t => t.RegionID, cascadeDelete: true)
                .Index(t => t.RegionID);
            
            CreateTable(
                "dbo.Region",
                c => new
                    {
                        RegionID = c.Guid(nullable: false),
                        RegionDescription = c.String(nullable: false, maxLength: 50, fixedLength: true),
                    })
                .PrimaryKey(t => t.RegionID)
                .Index(t => t.RegionDescription, unique: true);
            
            CreateTable(
                "dbo.Shippers",
                c => new
                    {
                        ShipperID = c.Guid(nullable: false),
                        CompanyName = c.String(nullable: false, maxLength: 40),
                        Phone = c.String(maxLength: 35),
                    })
                .PrimaryKey(t => t.ShipperID)
                .Index(t => t.CompanyName, unique: true);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierID = c.Guid(nullable: false),
                        CompanyName = c.String(nullable: false, maxLength: 40),
                        ContactName = c.String(maxLength: 35),
                        ContactTitle = c.String(maxLength: 35),
                        Address = c.String(maxLength: 60),
                        City = c.String(maxLength: 35),
                        Region = c.String(maxLength: 35),
                        PostalCode = c.String(maxLength: 35),
                        Country = c.String(maxLength: 35),
                        Phone = c.String(maxLength: 35),
                        Fax = c.String(maxLength: 35),
                        HomePage = c.String(),
                    })
                .PrimaryKey(t => t.SupplierID);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactID = c.Guid(nullable: false),
                        ContactType = c.String(maxLength: 50),
                        CompanyName = c.String(nullable: false, maxLength: 40),
                        ContactName = c.String(maxLength: 35),
                        ContactTitle = c.String(maxLength: 35),
                        Address = c.String(maxLength: 60),
                        City = c.String(maxLength: 35),
                        Region = c.String(maxLength: 35),
                        PostalCode = c.String(maxLength: 35),
                        Country = c.String(maxLength: 35),
                        Phone = c.String(maxLength: 35),
                        Extension = c.String(maxLength: 35),
                        Fax = c.String(maxLength: 35),
                        HomePage = c.String(),
                        PhotoPath = c.String(maxLength: 255),
                        Photo = c.Binary(),
                    })
                .PrimaryKey(t => t.ContactID);
            
            CreateTable(
                "dbo.TerritoryEmployees",
                c => new
                    {
                        Territory_TerritoryID = c.Guid(nullable: false),
                        Employee_EmployeeID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Territory_TerritoryID, t.Employee_EmployeeID })
                .ForeignKey("dbo.Territories", t => t.Territory_TerritoryID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.Employee_EmployeeID, cascadeDelete: true)
                .Index(t => t.Territory_TerritoryID)
                .Index(t => t.Employee_EmployeeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Invoices", "ShipperID", "dbo.Shippers");
            DropForeignKey("dbo.Invoices", "RegionID", "dbo.Region");
            DropForeignKey("dbo.Invoices", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Invoices", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Invoices", "ShipperID", "dbo.Employees");
            DropForeignKey("dbo.Invoices", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Orders", "ShipVia", "dbo.Shippers");
            DropForeignKey("dbo.Orders", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Territories", "RegionID", "dbo.Region");
            DropForeignKey("dbo.TerritoryEmployees", "Employee_EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.TerritoryEmployees", "Territory_TerritoryID", "dbo.Territories");
            DropForeignKey("dbo.Employees", "ReportsTo", "dbo.Employees");
            DropForeignKey("dbo.Orders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropIndex("dbo.TerritoryEmployees", new[] { "Employee_EmployeeID" });
            DropIndex("dbo.TerritoryEmployees", new[] { "Territory_TerritoryID" });
            DropIndex("dbo.Shippers", new[] { "CompanyName" });
            DropIndex("dbo.Region", new[] { "RegionDescription" });
            DropIndex("dbo.Territories", new[] { "RegionID" });
            DropIndex("dbo.Employees", new[] { "ReportsTo" });
            DropIndex("dbo.Orders", new[] { "ShipVia" });
            DropIndex("dbo.Orders", new[] { "EmployeeID" });
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            DropIndex("dbo.Invoices", new[] { "RegionID" });
            DropIndex("dbo.Invoices", new[] { "ProductID" });
            DropIndex("dbo.Invoices", new[] { "ShipperID" });
            DropIndex("dbo.Invoices", new[] { "OrderID" });
            DropIndex("dbo.Invoices", new[] { "CustomerID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.Products", new[] { "SupplierID" });
            DropTable("dbo.TerritoryEmployees");
            DropTable("dbo.Contacts");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Shippers");
            DropTable("dbo.Region");
            DropTable("dbo.Territories");
            DropTable("dbo.Employees");
            DropTable("dbo.Orders");
            DropTable("dbo.Customers");
            DropTable("dbo.Invoices");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
