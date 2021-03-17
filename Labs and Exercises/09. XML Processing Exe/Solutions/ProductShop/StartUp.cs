using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json.Converters;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Export.GetSoldProductsTask;
using ProductShop.Dtos.Export.GetUsersWithProducts;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private const string DatasetsDirectoryPath = "../../../Datasets";

        private const string ResultsDirectoryPath = "../../../Datasets/Results";
        public static void ResetDb(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database deleted successfully");
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully");
        }
        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<ProductShopProfile>(); });
        }
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            //ResetDb(context);

            InitializeMapper();

            //// 1. Import Users
            //string xml1 = File.ReadAllText($"{DatasetsDirectoryPath}/users.xml");
            //Console.WriteLine(ImportUsers(context, xml1));

            ////2. Import Products
            //string xml2 = File.ReadAllText($"{DatasetsDirectoryPath}/products.xml");
            //Console.WriteLine(ImportProducts(context, xml2));

            ////3. Import Categories
            //string xml3 = File.ReadAllText($"{DatasetsDirectoryPath}/categories.xml");
            //Console.WriteLine(ImportCategories(context, xml3));

            ////4. Import Categories and Products
            //string xml4 = File.ReadAllText($"{DatasetsDirectoryPath}/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, xml4));

            ////5. Products In Range
            //var result1 = GetProductsInRange(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/products-in-range.xml", result1);

            ////6. Sold Products
            //var result2 = GetSoldProducts(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/users-sold-products.xml", result2);

            ////7. Categories By Products Count
            //var result3 = GetCategoriesByProductsCount(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/categories-by-products.xml", result3);

            //8. Users and Products
            var result4 = GetUsersWithProducts(context);
            File.WriteAllText($"{ResultsDirectoryPath}/users-and-products.xml", result4);

        }
        //8. Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            /*Select users who have at least 1 sold product.
             Order them by the number of sold products (from highest to lowest).
             Select only their first and last name, age, count of sold products and for each product - name and price sorted by price (descending).
             Take top 10 records.*/
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var users = new UserRootDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any(p => p.Buyer != null)),
                Users = context.Users
                    .AsEnumerable() // or else inmemory error from judge
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Take(10)
                    .Select(u => new UserExportDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new SoldProductsDto()
                        {
                            Count = u.ProductsSold.Count(ps => ps.Buyer != null),
                            Products = u.ProductsSold
                                .Where(ps => ps.Buyer != null)
                                .Select(ps => new ExportProductSoldDto()
                                {
                                    Name = ps.Name,
                                    Price = ps.Price
                                })
                                .OrderByDescending(p => p.Price)
                                .ToList()
                        }
                    })
                    .ToList()
            };

            XmlSerializer serializer = new XmlSerializer(typeof(UserRootDto), new XmlRootAttribute("Users"));

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();

        }
        //7. Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            /*Get all categories. For each category select its name, the number of products, the average price of those products
             and the total revenue (total price sum) of those products (regardless if they have a buyer or not).
             Order them by the number of products (descending) then by total revenue.*/
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var categories = context
                .Categories
                .Select(x => new ExportCategoryByProductsDto()
                {
                    Name = x.Name,
                    AveragePrice = x.CategoryProducts.Average(s => s.Product.Price),
                    ProductsCount = x.CategoryProducts.Count,
                    TotalRevenue = x.CategoryProducts.Sum(s => s.Product.Price)
                })
                .OrderByDescending(c => c.ProductsCount)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportCategoryByProductsDto>), new XmlRootAttribute("Categories"));

            serializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }
        //6. Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            /*Get all users who have at least 1 sold item. Order them by last name, then by first name.
             Select the person's first and last name.
             For each of the sold products, select the product's name and price.
             Take top 5 records. */
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(x => x.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(x => new ExportUserSoldProductsDto()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new ExportSoldProductsDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                })
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportUserSoldProductsDto>), new XmlRootAttribute("Users"));

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }
        //5. Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            /*Get all products in a specified price range between 500 and 1000 (inclusive).
             Order them by price (from lowest to highest).
             Select only the product name, price and the full name of the buyer.
             Take top 10 records.*/
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                //.ProjectTo<ExportProductsInRangeDto>() no mapper sadge
                .Select(p => new ExportProductsInRangeDto()
                {
                    Name = p.Name,
                    BuyerName = p.Buyer.FirstName + " " + p.Buyer.LastName,
                    Price = p.Price
                })
                .Take(10)
                .ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportProductsInRangeDto>), new XmlRootAttribute("Products"));

            serializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }
        //4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            /*Import the categories and products ids from the provided file categories-products.xml.
             If provided category or product id, doesn’t exists, skip the whole entry!*/

            //judge doesnt like automapper so i wont bother making a map

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ImportCategoriesProductsDto>), new XmlRootAttribute("CategoryProducts"));

            var categoriesProductsDtos = (List<ImportCategoriesProductsDto>)xmlSerializer.Deserialize(new StringReader(inputXml));

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var categoryProductDto in categoriesProductsDtos)
            {
                if (context.Categories.Any(c => c.Id == categoryProductDto.CategoryId) && // check if both exist
                    context.Products.Any(p => p.Id == categoryProductDto.ProductId))
                {
                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = categoryProductDto.CategoryId,
                        ProductId = categoryProductDto.ProductId
                    };
                    categoryProducts.Add(categoryProduct);
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }
        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            /*Import the categories from the provided file categories.xml. 
            Some of the names will be null, so you don’t have to add them in the database.
            Just skip the record and continue.*/

            Mapper.Initialize(cfg => { cfg.AddProfile<ProductShopProfile>(); });

            var serializer = new XmlSerializer(typeof(List<ImportCategoriesDto>), new XmlRootAttribute("Categories"));

            var categoriesDtos = (List<ImportCategoriesDto>)serializer.Deserialize(new StringReader(inputXml));

            List<Category> categories = new List<Category>();
            // judge no like map
            //var categories = Mapper.Map<List<Category>>(categoriesDtos)
            //    .Where(c => c.Name != null)
            //    .ToList();
            foreach (var categoriesDto in categoriesDtos)
            {
                Category category = new Category()
                {
                    Name = categoriesDto.Name
                };
                if (category.Name != null) // in the xml none of them are null but ok
                {
                    categories.Add(category);
                }
            }

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            //Import the products from the provided file products.xml.
            var serializer = new XmlSerializer(typeof(List<ImportProductsDto>), new XmlRootAttribute("Products"));

            var productsDtos = (List<ImportProductsDto>)serializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();
            // judge doesnt like the mapper i guess
            //var products = Mapper.Map<List<Product>>(productsDtos);
            foreach (var productsDto in productsDtos)
            {
                var product = new Product()
                {
                    Name = productsDto.Name,
                    Price = productsDto.Price,
                    SellerId = productsDto.SellerId,
                    BuyerId = productsDto.BuyerId
                };
                products.Add(product);
            }

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
        // 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            //Import the users from the provided file users.xml.
            var serializer = new XmlSerializer(typeof(List<ImportUsersDto>), new XmlRootAttribute("Users"));

            var usersDtos = (List<ImportUsersDto>)serializer.Deserialize(new StringReader(inputXml));

            var users = Mapper.Map<List<User>>(usersDtos);

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}