using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftSyncPoC.EF.Client.Models;
using MicrosoftSyncPoC.Repository.EF.Models;
using Category = MicrosoftSyncPoC.Repository.EF.Models.Category;
using Contact = MicrosoftSyncPoC.Repository.EF.Models.Contact;
using Customer = MicrosoftSyncPoC.Repository.EF.Models.Customer;
using Employee = MicrosoftSyncPoC.Repository.EF.Models.Employee;
using Invoice = MicrosoftSyncPoC.Repository.EF.Models.Invoice;
using Order = MicrosoftSyncPoC.Repository.EF.Models.Order;
using Product = MicrosoftSyncPoC.Repository.EF.Models.Product;
using Region = MicrosoftSyncPoC.Repository.EF.Models.Region;
using Shipper = MicrosoftSyncPoC.Repository.EF.Models.Shipper;
using Supplier = MicrosoftSyncPoC.Repository.EF.Models.Supplier;
using Territory = MicrosoftSyncPoC.Repository.EF.Models.Territory;

namespace IntegrationTest
{
    [TestClass]
    public class SyncIntegrationTest
    {
        [TestMethod]
        public void Sync_Insert_Success_Master_To_Client()
        {
            var regionId = Guid.Empty;
            var territoryId = Guid.Empty;
            var shipperId = Guid.Empty;
            var supplierId = Guid.Empty;
            var categoryId = Guid.Empty;
            var contactId = Guid.Empty;
            var customerId = Guid.Empty;
            var employeeId = Guid.Empty;
            var productId = Guid.Empty;
            var orderId = Guid.Empty;
            var invoiceId = Guid.Empty;
            var randy = new Random();
            string shipperCompanyName;

            // 0. Delete stuff
            using (var context = new northwindContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);
                context.SaveChanges();

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();
            }

            using (var context = new northwind_guid_clientContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();

                context.SaveChanges();
            }

            var helperDel = new SynchronizationTestHelper();
            var syncResultsDel = helperDel.Synchronize();


            var local = "Server ";
            var counter = 0;
            // 1. Setup - insert test data into master table
            using (var context = new northwindContext())
            {
                var r = new Region {RegionID = Guid.NewGuid(), RegionDescription = local + "TestnaRegija " + counter};
                regionId = r.RegionID;

                var t = new Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;

                var s = new Shipper
                {
                    CompanyName = local + "TestCompanyName " + randy.Next() + counter,
                    ShipperID = Guid.NewGuid(),
                    Phone = "+385953956463"
                };
                shipperId = s.ShipperID;
                shipperCompanyName = s.CompanyName;

                var sup = new Supplier
                {
                    Address = local + "Testna Adresa " + counter,
                    City = local + "Zagreb " + counter,
                    CompanyName = local + "Testna Tvrtka d.o.o. " + counter,
                    ContactName = local + "Test Ime kontakta " + counter,
                    ContactTitle = local + "Test Contact Title " + counter,
                    Country = local + "Croatia " + counter,
                    Fax = "+385953956463",
                    HomePage = "https:\\www.google.com",
                    Phone = "+385953956463",
                    PostalCode = "10000",
                    SupplierID = Guid.NewGuid(),
                    Region = local + "Reg " + counter
                };
                supplierId = sup.SupplierID;

                var cat = new Category
                {
                    CategoryID = Guid.NewGuid(),
                    CategoryName = local + "Alk " + counter,
                    Description = local + "popit " + counter,
                    Picture = new byte[] {10, 2}
                };
                categoryId = cat.CategoryID;

                var con = new Contact
                {
                    ContactID = Guid.NewGuid(),
                    Address = local + "Adresa Test " + counter,
                    City = local + "Zagreb + counter ",
                    CompanyName = local + "Naziv tvrtke " + counter,
                    ContactTitle = local + "Titula kontakta " + counter,
                    ContactType = local + "Tip kontakta " + counter,
                    Country = local + "Croatia " + counter,
                    Extension = "385",
                    Fax = "+385953956463",
                    HomePage = "https:\\www.google.com",
                    Phone = "+385953956463",
                    Photo = new byte[] {29, 30, 1},
                    PhotoPath = "http://wvdl.net/vrora/sxqkl.xls",
                    PostalCode = "10000",
                    Region = local + "Test Regija " + counter
                };
                contactId = con.ContactID;

                var cust = new Customer
                {
                    Address = local + "Moja Testna Adresa " + counter,
                    City = local + "Zagreb " + counter,
                    CompanyName = local + "KingICT " + counter,
                    ContactTitle = local + "Titula kontakta " + counter,
                    Country = local + "Croatia " + counter,
                    CustomerID = Guid.NewGuid(),
                    Fax = "+385953956463",
                    Phone = "+385953956463",
                    PostalCode = "10000",
                    Region = local + "Testna Regija " + counter,
                    ContactName = local + "Naziv kontakta " + counter
                };
                customerId = cust.CustomerID;

                var emp = new Employee
                {
                    Address = local + "Moja Testna Adresa " + counter,
                    BirthDate = new DateTime(1979, 07, 30),
                    City = local + "Zagreb " + counter,
                    Country = local + "Croatia " + counter,
                    EmployeeID = Guid.NewGuid(),
                    Extension = "385",
                    FirstName = local + "TestIme " + counter,
                    HireDate = new DateTime(2014, 07, 30),
                    HomePhone = "+385953956463",
                    LastName = local + "TestPrezime " + counter,
                    Notes = local + "Ovo je testni uzorak " + counter,
                    Photo = new byte[] {1, 2, 3},
                    PhotoPath = "https://idaal.neta2/zyhxd/umdfb/ccsbq.doc",
                    PostalCode = "10000",
                    Region = local + "Testna Regija " + counter,
                    ReportsTo = null,
                    Title = local + "Testni title " + counter,
                    TitleOfCourtesy = local + "ToC " + counter
                };
                employeeId = emp.EmployeeID;

                var prod = new Product
                {
                    CategoryID = cat.CategoryID,
                    Discontinued = false,
                    ProductID = Guid.NewGuid(),
                    ProductName = local + "TestProductName " + counter,
                    QuantityPerUnit = "1",
                    ReorderLevel = 1,
                    SupplierID = sup.SupplierID,
                    UnitPrice = 100,
                    UnitsInStock = 100,
                    UnitsOnOrder = 1000
                };
                productId = prod.ProductID;

                var ord = new Order
                {
                    CustomerID = cust.CustomerID,
                    EmployeeID = emp.EmployeeID,
                    Freight = 100304,
                    OrderDate = DateTime.Now,
                    OrderID = Guid.NewGuid(),
                    RequiredDate = DateTime.Now,
                    ShipAddress = local + "TestShipAddress " + counter,
                    ShipCity = local + "TestShipCity " + counter,
                    ShipCountry = local + "TestShipCountry " + counter,
                    ShipName = local + "TestShipName " + counter,
                    ShipPostalCode = local + "TestShipPostalCode " + counter,
                    ShipRegion = local + "TestShipRegion " + counter,
                    ShipVia = s.ShipperID,
                    ShippedDate = DateTime.Now
                };
                orderId = ord.OrderID;

                var inv = new Invoice
                {
                    Address = local + "TestAddress" + counter,
                    City = "Zagreb",
                    Country = "Croatia",
                    CustomerID = cust.CustomerID,
                    CustomerName = local + "TestCustomerName" + counter,
                    Discount = 430404,
                    ExtendedPrice = 3333,
                    Freight = 432,
                    OrderDate = DateTime.Now,
                    OrderID = ord.OrderID,
                    PostalCode = "10000",
                    ProductID = prod.ProductID,
                    ProductName = local + "TestProductName " + counter,
                    Quantity = 88,
                    RegionID = r.RegionID,
                    RequiredDate = DateTime.Now,
                    SalespersonID = emp.EmployeeID,
                    ShipAddress = local + "TestShipAddress " + counter,
                    ShipCity = "Zagreb",
                    ShipCountry = "Croatia",
                    ShipPostalCode = "10000",
                    ShipRegion = local + "TestShipRegion " + counter,
                    ShippedDate = DateTime.Now,
                    ShipperName = local + "TestShipperName " + counter,
                    UnitPrice = 324,
                    ShipperID = emp.EmployeeID,
                    InvoiceID = Guid.NewGuid()
                };


                context.Regions.Add(r);
                context.Territories.Add(t);
                context.Shippers.Add(s);
                context.Suppliers.Add(sup);
                context.Categories.Add(cat);
                context.Contacts.Add(con);
                context.Customers.Add(cust);
                context.Employees.Add(emp);
                context.Products.Add(prod);
                context.Orders.Add(ord);
                context.SaveChanges();
                //  context.Invoices.Add(inv);
                //  context.SaveChanges();
            }


            // 2. Start sync process from client
            var helper = new SynchronizationTestHelper();
            var syncResults = helper.Synchronize();

            // We are expecting all 10 rows to be downloaded from the server
            Assert.AreEqual(syncResults.Stats.DownloadChangesTotal, 10);

            // 3. Check whether the entites have been successfully synced, i.e. are present in the local (client) db
            MicrosoftSyncPoC.EF.Client.Models.Region region;
            MicrosoftSyncPoC.EF.Client.Models.Territory territory;
            MicrosoftSyncPoC.EF.Client.Models.Shipper shipper;
            MicrosoftSyncPoC.EF.Client.Models.Supplier supplier;
            MicrosoftSyncPoC.EF.Client.Models.Category category;
            MicrosoftSyncPoC.EF.Client.Models.Contact contact;
            MicrosoftSyncPoC.EF.Client.Models.Customer customer;
            MicrosoftSyncPoC.EF.Client.Models.Employee employee;
            MicrosoftSyncPoC.EF.Client.Models.Product product;
            MicrosoftSyncPoC.EF.Client.Models.Order order;
            var invoice = "TO BE CONTINUED";


            using (var clientContext = new northwind_guid_clientContext())
            {
                region = clientContext.Regions.Find(regionId);
                territory = clientContext.Territories.Find(territoryId);
                shipper = clientContext.Shippers.Find(shipperId);
                supplier = clientContext.Suppliers.Find(supplierId);
                category = clientContext.Categories.Find(categoryId);
                contact = clientContext.Contacts.Find(contactId);
                customer = clientContext.Customers.Find(customerId);
                employee = clientContext.Employees.Find(employeeId);
                product = clientContext.Products.Find(productId);
                order = clientContext.Orders.Find(orderId);
                invoice = "TO BE CONTINUED";
            }

            // We are expecting to find these entities copied to the client database...Ids will of course be different as per sync requirements...
            Assert.AreEqual(region.RegionDescription.TrimEnd(), local + "TestnaRegija " + counter);
            Assert.AreEqual(territory.TerritoryDescription.TrimEnd(), local + "TestniTeritorij " + counter);
            Assert.AreEqual(shipper.CompanyName.TrimEnd(), shipperCompanyName);
        }

        [TestMethod]
        public void Sync_Insert_Success_Client_To_Master()
        {
            var regionId = Guid.Empty;
            var territoryId = Guid.Empty;
            var shipperId = Guid.Empty;
            var supplierId = Guid.Empty;
            var categoryId = Guid.Empty;
            var contactId = Guid.Empty;
            var customerId = Guid.Empty;
            var employeeId = Guid.Empty;
            var productId = Guid.Empty;
            var orderId = Guid.Empty;
            var invoiceId = Guid.Empty;
            var randy = new Random();

            // 0. Delete stuff
            using (var context = new northwindContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);
                context.SaveChanges();

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();
            }

            using (var context = new northwind_guid_clientContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();

                context.SaveChanges();
            }

            var helperDel = new SynchronizationTestHelper();
            var syncResultsDel = helperDel.Synchronize();
            // 1. Setup - insert test data into client table
            var local = "Client ";
            var counter = 0;

            using (var clientContext = new northwind_guid_clientContext())
            {
                var r = new MicrosoftSyncPoC.EF.Client.Models.Region
                {
                    RegionID = Guid.NewGuid(),
                    RegionDescription = local + "TestnaRegija " + counter
                };
                regionId = r.RegionID;

                var t = new MicrosoftSyncPoC.EF.Client.Models.Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;

                var s = new MicrosoftSyncPoC.EF.Client.Models.Shipper
                {
                    CompanyName = local + "TestCompanyName " + randy.Next() + counter,
                    ShipperID = Guid.NewGuid(),
                    Phone = "+385953956463"
                };
                shipperId = s.ShipperID;
                var shipperCompanyName = s.CompanyName;

                var sup = new MicrosoftSyncPoC.EF.Client.Models.Supplier
                {
                    Address = local + "Testna Adresa " + counter,
                    City = local + "Zagreb " + counter,
                    CompanyName = local + "Testna Tvrtka d.o.o. " + counter,
                    ContactName = local + "Test Ime kontakta " + counter,
                    ContactTitle = local + "Test Contact Title " + counter,
                    Country = local + "Croatia " + counter,
                    Fax = "+385953956463",
                    HomePage = "https:\\www.google.com",
                    Phone = "+385953956463",
                    PostalCode = "10000",
                    SupplierID = Guid.NewGuid(),
                    Region = local + "Reg " + counter
                };
                supplierId = sup.SupplierID;

                var cat = new MicrosoftSyncPoC.EF.Client.Models.Category
                {
                    CategoryID = Guid.NewGuid(),
                    CategoryName = local + "Alk " + counter,
                    Description = local + "popit " + counter,
                    Picture = new byte[] {10, 2}
                };
                categoryId = cat.CategoryID;

                var con = new MicrosoftSyncPoC.EF.Client.Models.Contact
                {
                    ContactID = Guid.NewGuid(),
                    Address = local + "Adresa Test " + counter,
                    City = local + "Zagreb + counter ",
                    CompanyName = local + "Naziv tvrtke " + counter,
                    ContactTitle = local + "Titula kontakta " + counter,
                    ContactType = local + "Tip kontakta " + counter,
                    Country = local + "Croatia " + counter,
                    Extension = "385",
                    Fax = "+385953956463",
                    HomePage = "https:\\www.google.com",
                    Phone = "+385953956463",
                    Photo = new byte[] {29, 30, 1},
                    PhotoPath = "http://wvdl.net/vrora/sxqkl.xls",
                    PostalCode = "10000",
                    Region = local + "Test Regija " + counter
                };
                contactId = con.ContactID;

                var cust = new MicrosoftSyncPoC.EF.Client.Models.Customer
                {
                    Address = local + "Moja Testna Adresa " + counter,
                    City = local + "Zagreb " + counter,
                    CompanyName = local + "KingICT " + counter,
                    ContactTitle = local + "Titula kontakta " + counter,
                    Country = local + "Croatia " + counter,
                    CustomerID = Guid.NewGuid(),
                    Fax = "+385953956463",
                    Phone = "+385953956463",
                    PostalCode = "10000",
                    Region = local + "Testna Regija " + counter,
                    ContactName = local + "Naziv kontakta " + counter
                };
                customerId = cust.CustomerID;

                var emp = new MicrosoftSyncPoC.EF.Client.Models.Employee
                {
                    Address = local + "Moja Testna Adresa " + counter,
                    BirthDate = new DateTime(1979, 07, 30),
                    City = local + "Zagreb " + counter,
                    Country = local + "Croatia " + counter,
                    EmployeeID = Guid.NewGuid(),
                    Extension = "385",
                    FirstName = local + "TestIme " + counter,
                    HireDate = new DateTime(2014, 07, 30),
                    HomePhone = "+385953956463",
                    LastName = local + "TestPrezime " + counter,
                    Notes = local + "Ovo je testni uzorak " + counter,
                    Photo = new byte[] {1, 2, 3},
                    PhotoPath = "https://idaal.neta2/zyhxd/umdfb/ccsbq.doc",
                    PostalCode = "10000",
                    Region = local + "Testna Regija " + counter,
                    ReportsTo = null,
                    Title = local + "Testni title " + counter,
                    TitleOfCourtesy = local + "ToC " + counter
                };
                employeeId = emp.EmployeeID;

                var prod = new MicrosoftSyncPoC.EF.Client.Models.Product
                {
                    CategoryID = cat.CategoryID,
                    Discontinued = false,
                    ProductID = Guid.NewGuid(),
                    ProductName = local + "TestProductName " + counter,
                    QuantityPerUnit = "1",
                    ReorderLevel = 1,
                    SupplierID = sup.SupplierID,
                    UnitPrice = 100,
                    UnitsInStock = 100,
                    UnitsOnOrder = 1000
                };
                productId = prod.ProductID;

                var ord = new MicrosoftSyncPoC.EF.Client.Models.Order
                {
                    CustomerID = cust.CustomerID,
                    EmployeeID = emp.EmployeeID,
                    Freight = 100304,
                    OrderDate = DateTime.Now,
                    OrderID = Guid.NewGuid(),
                    RequiredDate = DateTime.Now,
                    ShipAddress = local + "TestShipAddress " + counter,
                    ShipCity = local + "TestShipCity " + counter,
                    ShipCountry = local + "TestShipCountry " + counter,
                    ShipName = local + "TestShipName " + counter,
                    ShipPostalCode = local + "TestShipPostalCode " + counter,
                    ShipRegion = local + "TestShipRegion " + counter,
                    ShipVia = s.ShipperID,
                    ShippedDate = DateTime.Now
                };
                orderId = ord.OrderID;

                var inv = new MicrosoftSyncPoC.EF.Client.Models.Invoice
                {
                    Address = local + "TestAddress" + counter,
                    City = "Zagreb",
                    Country = "Croatia",
                    CustomerID = cust.CustomerID,
                    CustomerName = local + "TestCustomerName" + counter,
                    Discount = 430404,
                    ExtendedPrice = 3333,
                    Freight = 432,
                    OrderDate = DateTime.Now,
                    OrderID = ord.OrderID,
                    PostalCode = "10000",
                    ProductID = prod.ProductID,
                    ProductName = local + "TestProductName " + counter,
                    Quantity = 88,
                    RegionID = r.RegionID,
                    RequiredDate = DateTime.Now,
                    SalespersonID = emp.EmployeeID,
                    ShipAddress = local + "TestShipAddress " + counter,
                    ShipCity = "Zagreb",
                    ShipCountry = "Croatia",
                    ShipPostalCode = "10000",
                    ShipRegion = local + "TestShipRegion " + counter,
                    ShippedDate = DateTime.Now,
                    ShipperName = local + "TestShipperName " + counter,
                    UnitPrice = 324,
                    ShipperID = emp.EmployeeID,
                    InvoiceID = Guid.NewGuid()
                };


                clientContext.Regions.Add(r);
                clientContext.Territories.Add(t);
                clientContext.Shippers.Add(s);
                clientContext.Suppliers.Add(sup);
                clientContext.Categories.Add(cat);
                clientContext.Contacts.Add(con);
                clientContext.Customers.Add(cust);
                clientContext.Employees.Add(emp);
                clientContext.Products.Add(prod);
                clientContext.Orders.Add(ord);
                clientContext.SaveChanges();
                //  clientContext.Invoices.Add(inv);
                //  clientContext.SaveChanges();


                // 2. Start sync process from client
                var helper = new SynchronizationTestHelper();
                var syncResults = helper.Synchronize();

                // We are expecting to see all 10 rows updated on the server
                Assert.AreEqual(syncResults.Stats.UploadChangesApplied, 10);

                // 3. Check whether the entites have been successfully synced, i.e. are present in the master db
                MicrosoftSyncPoC.EF.Client.Models.Region region;
                MicrosoftSyncPoC.EF.Client.Models.Territory territory;
                MicrosoftSyncPoC.EF.Client.Models.Shipper shipper;
                MicrosoftSyncPoC.EF.Client.Models.Supplier supplier;
                MicrosoftSyncPoC.EF.Client.Models.Category category;
                MicrosoftSyncPoC.EF.Client.Models.Contact contact;
                MicrosoftSyncPoC.EF.Client.Models.Customer customer;
                MicrosoftSyncPoC.EF.Client.Models.Employee employee;
                MicrosoftSyncPoC.EF.Client.Models.Product product;
                MicrosoftSyncPoC.EF.Client.Models.Order order;
                var invoice = "TO BE CONTINUED";


                using (var context = new northwindContext())
                {
                    region = clientContext.Regions.Find(regionId);
                    territory = clientContext.Territories.Find(territoryId);
                    shipper = clientContext.Shippers.Find(shipperId);
                    supplier = clientContext.Suppliers.Find(supplierId);
                    category = clientContext.Categories.Find(categoryId);
                    contact = clientContext.Contacts.Find(contactId);
                    customer = clientContext.Customers.Find(customerId);
                    employee = clientContext.Employees.Find(employeeId);
                    product = clientContext.Products.Find(productId);
                    order = clientContext.Orders.Find(orderId);
                    invoice = "TO BE CONTINUED";
                }

                // We are expecting to find these entities copied to the server database...Ids will of course be different as per sync requirements...
                Assert.AreEqual(region.RegionDescription, local + "TestnaRegija " + counter);
                Assert.AreEqual(territory.TerritoryDescription, local + "TestniTeritorij " + counter);
                Assert.AreEqual(shipper.CompanyName, shipperCompanyName);
            }
        }

        [TestMethod]
        public void Sync_Insert_Conflict_Client_To_Master()
        {
            // !!! REMOTE SUPERCEDES !!!

            var regionId = Guid.Empty;
            var territoryId = Guid.Empty;
            var shipperId = Guid.Empty;
            var randy = new Random();
            var thisEntityMustWin = Guid.Empty;
            var testClientPhone = "ClientPhone";
            var testServerPhone = "ServerPhone";
            // 0. Delete stuff
            using (var context = new northwindContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);
                context.SaveChanges();

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();
            }

            using (var context = new northwind_guid_clientContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();

                context.SaveChanges();
            }

            var helperDel = new SynchronizationTestHelper();
            var syncResultsDel = helperDel.Synchronize();

            // 1. Setup - insert test data into master table
            using (var context = new northwindContext())
            {
                var local = "Server ";
                var counter = 0;

                var r = new Region {RegionID = Guid.NewGuid(), RegionDescription = local + "TestnaRegija " + counter};
                regionId = r.RegionID;

                var t = new Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;

                var s = new Shipper
                {
                    CompanyName = local + "TestCompanyName " + randy.Next() + counter,
                    ShipperID = Guid.NewGuid(),
                    Phone = testServerPhone
                };
                shipperId = s.ShipperID;
                thisEntityMustWin = s.ShipperID;


                try
                {
                    // context.Regions.Add(r);
                    // context.Territories.Add(t);
                    context.Shippers.Add(s);
                    context.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }

            // 2. Insert the same data into Client database in order to create a conflict
            using (var clientContext = new northwind_guid_clientContext())
            {
                var local = "Client ";
                var counter = 0;

                var r = new MicrosoftSyncPoC.EF.Client.Models.Region
                {
                    RegionID = regionId,
                    RegionDescription = local + "TestnaRegija " + counter
                };

                var t = new MicrosoftSyncPoC.EF.Client.Models.Territory
                {
                    RegionID = regionId,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = territoryId
                };

                var s = new MicrosoftSyncPoC.EF.Client.Models.Shipper
                {
                    CompanyName = local + "TestName " + randy.Next() + counter,
                    ShipperID = shipperId,
                    Phone = testClientPhone
                };

                try
                {
                    // clientContext.Regions.Add(r);
                    // clientContext.Territories.Add(t);
                    clientContext.Shippers.Add(s);
                    clientContext.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }


            // 2. Start sync process from client
            var helper = new SynchronizationTestHelper();
            var syncResults = helper.Synchronize();

            MicrosoftSyncPoC.EF.Client.Models.Shipper shipper;
            // 3. Check whether the entites have been successfully synced, in this case the server should have won the "clientInsertServerInsert" conflict...
            using (var clientContext = new northwind_guid_clientContext())
            {
                shipper = clientContext.Shippers.Find(shipperId);
            }

            // In this setup we are expecting the server value to win the conflict
            Assert.AreEqual(shipper.ShipperID, thisEntityMustWin);
            Assert.AreEqual(shipper.Phone, testServerPhone);
        }

        [TestMethod]
        public void Sync_Constraint_Conflict_Client_To_Master()
        {
            var regionId = Guid.Empty;
            var territoryId = Guid.Empty;
            var shipperId = Guid.Empty;
            Guid shipperIdMaster;
            Guid shipperIdClient;

            // 0. Delete stuff
            using (var context = new northwindContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);
                context.SaveChanges();

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();
            }
            using (var context = new northwind_guid_clientContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();

                context.SaveChanges();
            }
            var helperDel = new SynchronizationTestHelper();
            var syncResultsDel = helperDel.Synchronize();


            // 1. Setup - insert test data into Master database, one of the entities has a unique constraint on one of the attributes


            using (var context = new northwindContext())
            {
                var local = "Server ";
                var counter = 0;

                var r = new Region
                {
                    RegionID = Guid.NewGuid(),
                    RegionDescription = local + "TestnaRegija " + counter
                };

                var t = new Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };

                var s = new Shipper
                {
                    CompanyName = "UniqueConstraintOnCompanyName",
                    ShipperID = Guid.NewGuid(),
                    Phone = "ServerShipperPhone"
                };

                shipperIdMaster = s.ShipperID;

                context.Regions.Add(r);
                context.Territories.Add(t);
                context.Shippers.Add(s);
                context.SaveChanges();
            }


            // 2. Insert some data into Client database in order to create a constraint conflict

            using (var context = new northwind_guid_clientContext())
            {
                var local = "Client ";
                var counter = 0;

                var r = new MicrosoftSyncPoC.EF.Client.Models.Region
                {
                    RegionID = Guid.NewGuid(),
                    RegionDescription = local + "TestnaRegija" + counter
                };
                regionId = r.RegionID;

                var t = new MicrosoftSyncPoC.EF.Client.Models.Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;

                var s = new MicrosoftSyncPoC.EF.Client.Models.Shipper
                {
                    CompanyName = "UniqueConstraintOnCompanyName",
                    ShipperID = Guid.NewGuid(),
                    Phone = "ClientShipperPhone"
                };
                shipperIdClient = s.ShipperID;

                context.Regions.Add(r);
                context.Territories.Add(t);
                context.Shippers.Add(s);
                context.SaveChanges();
            }

            // 3. Start sync process from client

            var helper = new SynchronizationTestHelper();
            var syncResults = helper.Synchronize();


            // 4. Check whether the entites have been unsuccessfully synced, i.e. should not have been synced from client to master as they already exist

            Assert.AreEqual(syncResults.Stats.UploadChangesFailed, 1);
            Assert.AreEqual(syncResults.Stats.DownloadChangesFailed, 1);

            MicrosoftSyncPoC.EF.Client.Models.Shipper shipperClient;

            using (var clientContext = new northwind_guid_clientContext())
            {
                shipperClient = clientContext.Shippers.Find(shipperIdMaster);
            }

            Shipper shipperMaster;

            using (var context = new northwindContext())
            {
                shipperMaster = context.Shippers.Find(shipperIdClient);
            }

            // In this setup we are expecting to **not** find client shippers in the master Database due to unique constraint conflict
            Assert.IsNull(shipperClient);
            // In this setup we are expecting to **not** find master shippers in the client Database due to unique constraint conflict
            Assert.IsNull(shipperMaster);
        }

        [TestMethod]
        public void Sync_Update_Conflict_Client_Update_To_Master_Update()
        {
            // !!! LOCAL SUPERCEDES !!! (Server Wins)

            var regionId = Guid.Empty;
            var territoryId = Guid.Empty;
            var shipperId = Guid.Empty;
            var randy = new Random();

            // 0. Delete stuff
            using (var context = new northwindContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);
                context.SaveChanges();

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();
            }

            using (var context = new northwind_guid_clientContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();

                context.SaveChanges();
            }

            var helperDel = new SynchronizationTestHelper();
            var syncResultsDel = helperDel.Synchronize();

            // 1. Setup - insert test data into master table
            using (var context = new northwindContext())
            {
                var local = "Server ";
                var counter = 0;

                var r = new Region {RegionID = Guid.NewGuid(), RegionDescription = local + "TestnaRegija " + counter};
                regionId = r.RegionID;

                var t = new Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;

                var s = new Shipper
                {
                    CompanyName = local + "TestCompanyName " + randy.Next() + counter,
                    ShipperID = Guid.NewGuid(),
                    Phone = "+385953956463"
                };
                shipperId = s.ShipperID;


                try
                {
                    // context.Regions.Add(r);
                    // context.Territories.Add(t);
                    context.Shippers.Add(s);
                    context.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }

            // 2. Start sync process from client

            var helper = new SynchronizationTestHelper();
            var syncResults = helper.Synchronize();

            // Client will now have the same record as the Master

            // 3. Update both the master and the client record, and then sync them

            using (var context = new northwindContext())
            {
                var local = "Server ";
                var counter = 0;

                var shipper = context.Shippers.Find(shipperId);
                shipper.Phone = "MasterPhone";

                try
                {
                    context.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }

            using (var clientContext = new northwind_guid_clientContext())
            {
                var local = "Client ";
                var counter = 0;

                var shipper = clientContext.Shippers.Find(shipperId);
                shipper.Phone = "ClientPhone";

                try
                {
                    clientContext.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }

            var helper3 = new SynchronizationTestHelper();
            var syncResults3 = helper3.Synchronize();

            // 3. Check whether the entites have been unsuccessfully synced, i.e. should not have been synced from client to master as they already exist
            Assert.AreEqual(syncResults3.Stats.DownloadChangesApplied, 1);
            Assert.AreEqual(syncResults3.Stats.UploadChangesApplied, 1);


            MicrosoftSyncPoC.EF.Client.Models.Shipper shipperClient;

            using (var clientContext = new northwind_guid_clientContext())
            {
                shipperClient = clientContext.Shippers.Find(shipperId);
            }

            Shipper shipperMaster;

            using (var context = new northwindContext())
            {
                shipperMaster = context.Shippers.Find(shipperId);
            }

            // We expect to see both rows having "Master" data as Master database wins ClientUpdateServerUpdate conflicts...
            Assert.AreEqual(shipperClient.Phone, "MasterPhone");
            Assert.AreEqual(shipperMaster.Phone, "MasterPhone");
        }

        [TestMethod]
        public void Sync_Client_Inserts_Unique_Constraint_Conflict_Entity_With_Child_Entity_Server_Wins()
        {
            // Script: Server already contains parent entity that has a unique constraint on it, also has a child in a parent child relationship
            //         Client does not have knowledge od the servers parent entity nor child
            //         Client creates a new parent entity violating the unique constraint (on the server), and a child in a parent child relationship
            //         Upon sync we want to keep the server version (server wins) of the parent, but insert the child relation

            var regionId = Guid.Empty;
            var territoryId = Guid.Empty;
            var shipperId = Guid.Empty;
            var randy = new Random();

            // 0. Delete stuff
            using (var context = new northwindContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);
                context.SaveChanges();

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();
            }

            using (var context = new northwind_guid_clientContext())
            {
                var list = context.Regions.Where(i => i.RegionID != null).ToList();
                context.Regions.RemoveRange(list);
                context.SaveChanges();

                var list2 = context.Territories.Where(i => i.TerritoryID != null).ToList();
                context.Territories.RemoveRange(list2);
                context.SaveChanges();

                var list4 = context.Orders.Where(i => i.OrderID != null).ToList();
                context.Orders.RemoveRange(list4);

                var list3 = context.Shippers.Where(i => i.ShipperID != null).ToList();
                context.Shippers.RemoveRange(list3);
                context.SaveChanges();

                context.SaveChanges();
            }

            var helperDel = new SynchronizationTestHelper();
            var syncResultsDel = helperDel.Synchronize();

            // 1. Setup - insert test data into master table
            using (var context = new northwindContext())
            {
                var local = "Server ";
                var counter = 0;

                var r = new Region
                {
                    RegionID = Guid.NewGuid(),
                    RegionDescription = "UniqueConstraintRegionDescription"
                };
                regionId = r.RegionID;

                var t = new Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;


                try
                {
                    context.Regions.Add(r);
                    context.Territories.Add(t);
                    context.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }

            using (var clientContext = new northwind_guid_clientContext())
            {
                var local = "Client ";
                var counter = 0;

                var r = new MicrosoftSyncPoC.EF.Client.Models.Region
                {
                    RegionID = Guid.NewGuid(),
                    RegionDescription = "UniqueConstraintRegionDescription"
                };
                regionId = r.RegionID;

                var t = new MicrosoftSyncPoC.EF.Client.Models.Territory
                {
                    RegionID = r.RegionID,
                    TerritoryDescription = local + "TestniTeritorij " + counter,
                    TerritoryID = Guid.NewGuid()
                };
                territoryId = t.TerritoryID;


                try
                {
                    clientContext.Regions.Add(r);
                    clientContext.Territories.Add(t);
                    clientContext.SaveChanges();
                }
                catch (Exception entEx)
                {
                }
            }

            // 2. Start sync process from client

            var helper = new SynchronizationTestHelper();
            var syncResults = helper.Synchronize();


            MicrosoftSyncPoC.EF.Client.Models.Shipper shipperClient;

            using (var clientContext = new northwind_guid_clientContext())
            {
                shipperClient = clientContext.Shippers.Find(shipperId);
            }

            Shipper shipperMaster;

            using (var context = new northwindContext())
            {
                shipperMaster = context.Shippers.Find(shipperId);
            }

            // We expect to see both rows having "Master" data as Master database wins ClientUpdateServerUpdate conflicts...
            // Assert.AreEqual(shipperClient, "MasterPhone");
            // Assert.AreEqual(shipperMaster, "MasterPhone");
        }
    }
}