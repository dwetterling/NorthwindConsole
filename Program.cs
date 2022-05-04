using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Add a Product");
                    Console.WriteLine("6) Edit a product");
                    Console.WriteLine("7) View Products");
                    Console.WriteLine("8) View a product details");
                    Console.WriteLine("9) Edit a category");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NWConsole_48_DJWContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "2")
                    {
                        var db = new NWConsole_48_DJWContext();
                        Category category = InputCategory(db);
                        if (category != null){
                            
                            db.AddCategory(category);
                            logger.Info("Category added - {name}", category.CategoryName);
                        
                    }
                      }  else if (choice == "3")
                    {
                        var db = new NWConsole_48_DJWContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                         foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NWConsole_48_DJWContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    } else if (choice == "5"){
                        var db = new NWConsole_48_DJWContext();
                        Product product = InputProduct(db);
                        if (product != null){
                            
                            db.AddProduct(product);
                            logger.Info("Product added - {name}", product.ProductName);
                        }
                        
                    }else if (choice == "6"){
                        Console.WriteLine("Choose the product to edit:");
                        var db = new NWConsole_48_DJWContext();
                        var product = GetProducts(db);
                        if (product != null)
                        {
                           
                            Product UpdatedProduct = InputProduct(db);
                            if (UpdatedProduct != null)
                            {
                                UpdatedProduct.ProductId = product.ProductId;
                                db.EditProduct(UpdatedProduct);
                                logger.Info($"Product (id: {product.ProductId}) updated");
                            }
                        }
                    } else if (choice == "7"){
                        Console.WriteLine("How would you like to view the products?");
                        Console.WriteLine("1) All Produts");
                        Console.WriteLine("2) Discontinued Produts");
                        Console.WriteLine("3) Not Discontinued Produts");
                        
                        string view = Console.ReadLine();
                        logger.Info($"Option {view} selected");

                        if (view == "1"){
                            var db = new NWConsole_48_DJWContext();
                        var product = GetProductName(db);
                        }else if (view == "2"){
                            Console.WriteLine("Viewing all discontinued products");
                            var db = new NWConsole_48_DJWContext();
                        var product = GetDiscontinuedProducts(db);
                        }else if (view == "3"){
                            Console.WriteLine("Viewing all non discontinued products");
                            var db = new NWConsole_48_DJWContext();
                        var product = GetNonDiscontinuedProducts(db);
                        }
                        
                    }else if (choice == "8"){
                         Console.WriteLine("Choose the product to view details of:");
                        var db = new NWConsole_48_DJWContext();
                        var product = GetProducts(db);

                        logger.Info($"product {product.ProductId} selected");
                    if (product != null)
                            {
                                
                     Console.WriteLine($"Product id: {product.ProductId}");
                     Console.WriteLine($"Product Name: {product.ProductName}");
                     Console.WriteLine($"Product Supplier: {product.SupplierId}");
                     Console.WriteLine($"Product Category ID: {product.CategoryId}");
                     Console.WriteLine($"Quantity per unit: {product.QuantityPerUnit}");
                     Console.WriteLine($"Unit Price: {product.UnitPrice}");
                     Console.WriteLine($"Units in stock: {product.UnitsInStock}");
                     Console.WriteLine($"Units on order: {product.UnitsOnOrder}");
                     Console.WriteLine($"Product reorder level: {product.ReorderLevel}");
                     Console.WriteLine($"Product discontinued: {product.Discontinued} ");                       
                            }
                    Console.WriteLine();
                    }else if (choice == "9"){
                        Console.WriteLine("Choose the category to edit:");
                        var db = new NWConsole_48_DJWContext();
                        var category = GetCategories(db);
                        if (category != null)
                        {
                           
                            Category UpdatedCategory = InputCategory(db);
                            if (UpdatedCategory != null)
                            {
                                UpdatedCategory.CategoryId = category.CategoryId;
                                db.EditCategory(UpdatedCategory);
                                logger.Info($"Category (id: {category.CategoryId}) updated");
                            }
                        }
                    }
                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }

        public static Category InputCategory(NWConsole_48_DJWContext db){
            Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            
                            
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                
                                /* db.AddCategory(category);
                                logger.Info("Category added - {name}", category.CategoryName); */
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }return category;
        }
        public static Category GetCategories(NWConsole_48_DJWContext db)
        {
            
            var categories = db.Categories.OrderBy(b => b.CategoryId);
            foreach (Category b in categories)
            {
                Console.WriteLine($"{b.CategoryId}: {b.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category = db.Categories.FirstOrDefault(b => b.CategoryId == CategoryId);
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Category Id");
            return null;
        }
        public static Product InputProduct(NWConsole_48_DJWContext db)
        {
            Product product = new Product();
                        Console.WriteLine("Enter Product Name:");
                        product.ProductName = Console.ReadLine();
                        Console.WriteLine("Enter the Supplier ID:");
                        product.SupplierId = Convert.ToInt32(Console.ReadLine());    
                        Console.WriteLine("Enter the Category ID:"); 
                        product.CategoryId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter the Quantity per unit");
                        product.QuantityPerUnit = Console.ReadLine();
                        Console.WriteLine("Enter the Unit price");
                        product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
                        Console.WriteLine("Enter the Units in stock");
                        product.UnitsInStock = Convert.ToInt16(Console.ReadLine());  
                        Console.WriteLine("Enter the Units on order");
                        product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("Enter the Reorder level");
                        product.ReorderLevel = Convert.ToInt16(Console.ReadLine());  
                        Console.WriteLine("Enter if  or if not Discontinued");
                        product.Discontinued = Convert.ToBoolean(Console.ReadLine());                   
                        
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            
                            // check for unique name
                            if (db.Products.Any(c => c.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                // TODO: save category to db
                               
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }return product;
        }
        public static Product GetProducts(NWConsole_48_DJWContext db)
        {
            
            var products = db.Products.OrderBy(b => b.ProductId);
            foreach (Product b in products)
            {
                Console.WriteLine($"{b.ProductId}: {b.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(b => b.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Product GetProductName(NWConsole_48_DJWContext db)
        {
            
            var products = db.Products.OrderBy(b => b.ProductId);
            foreach (Product b in products)
            {
                Console.WriteLine($"{b.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(b => b.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Product GetDiscontinuedProducts(NWConsole_48_DJWContext db)
        {
            
            var products = db.Products.Where(b => b.Discontinued == true);
            foreach (Product b in products)
            {
                Console.WriteLine($"{b.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(b => b.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Product GetNonDiscontinuedProducts(NWConsole_48_DJWContext db)
        {
            
            var products = db.Products.Where(b => b.Discontinued == false);
            foreach (Product b in products)
            {
                Console.WriteLine($"{b.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(b => b.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Product GetProduct(NWConsole_48_DJWContext db)
        {
            
            var products = db.Products.OrderBy(b => b.ProductId);
            foreach (Product b in products)
            {
                Console.WriteLine($"{b.ProductId}: {b.ProductName}, {b.SupplierId}, {b.CategoryId}, {b.QuantityPerUnit}, {b.UnitPrice}, {b.UnitsInStock}, {b.UnitsOnOrder}, {b.ReorderLevel}, {b.Discontinued}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(b => b.ProductId == ProductId);
                if (product != null)
                {

                    return product;
                    
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

    }
}