using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO;
using ProductShop.DTO.GetUsersWithProducts;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private const string DatasetsDirectoryPath = "../../../Datasets";

        private const string ResultsDirectoryPath = "../../../Datasets/Results";

        public static void Main(string[] args)
        {

            var context = new ProductShopContext();

            InitializeMapper();
            //ResetDataBase(context); // to avoid migrations

            ////01.Import Users
            //string usersJson = File.ReadAllText($"{DatasetsDirectoryPath}/users.json");
            //Console.WriteLine(ImportUsers(context, usersJson));

            ////02.Import Products
            //string productsJson = File.ReadAllText($"{DatasetsDirectoryPath}/products.json");
            //Console.WriteLine(ImportProducts(context, productsJson));

            ////03.Import Categories
            //string categoriesJson = File.ReadAllText($"{DatasetsDirectoryPath}/categories.json");
            //Console.WriteLine(ImportCategories(context, categoriesJson));

            ////04.Import Categories and Products
            //string categoriesProductsJson = File.ReadAllText($"{DatasetsDirectoryPath}/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsJson));

            // 05. Export Products in Range
            //var json = GetProductsInRange(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/products-in-range.json", json);

            // 06. Export Successfully Sold Products
            //var json = GetSoldProducts(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/users-sold-products.json", json);

            // 07. Export Categories by Products Count
            //var json = GetCategoriesByProductsCount(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/categories-by-products.json", json);

            // 08. Export Users and Products
            var json = GetUsersWithProducts(context);
            File.WriteAllText($"{ResultsDirectoryPath}/users-and-products.json", json);

        }
         //08. Export Users and Products // Too complicated with DTOs
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersProducts = context
                    .Users
                    .AsEnumerable()
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                    .Select(u => new
                    {
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        age = u.Age,
                        soldProducts = new
                        {
                            count = u.ProductsSold.Count(p => p.Buyer != null),
                            products = u.ProductsSold.Where(p => p.Buyer != null)
                            .Select(ps => new
                            {
                                name = ps.Name,
                                price = ps.Price
                            })
                            .ToList()
                        }
                    })
                    .ToList();

            var usersInfo = new
            {
                usersCount = usersProducts.Count,
                users = usersProducts
            };


            var json = JsonConvert.SerializeObject(usersInfo,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            return json;
        }

        // 08. Export Users and Products failed attempt
        //public static string GetUsersWithProducts(ProductShopContext context)
        //{
        //    /*Get all users who have at least 1 sold product with a buyer.
        //     Order them in descending order by the number of sold products with a buyer.
        //     Select only their first and last name, age and for each product - name and price.
        //     Ignore all null values.*/

        //    var users = context.Users
        //        .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
        //        .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
        //        .Select(u => new GetUsersWithProductsDTO()
        //        {
        //            FirstName = u.FirstName,
        //            LastName = u.LastName,
        //            Age = u.Age,
        //            SoldProducts = new GetSoldProductsDTO()
        //            {
        //                Count = u.ProductsSold.Count(p => p.Buyer != null),
        //                Products = u.ProductsSold
        //                    .ToList()
        //                    .Where(p => p.Buyer != null)
        //                    .Select(p => new GetProductsDTO()
        //                    {
        //                        Name = p.Name,
        //                        Price = p.Price
        //                    })
        //                    .ToList()
        //            }
        //        })
        //        .OrderByDescending(u => u.soldProducts.count)
        //        .ToList();

        //    var resultObj = new GetAllUsersDTO()
        //    {
        //        Count = users.Count,
        //        Users = users
        //    };

        //    var result = JsonConvert.SerializeObject(resultObj, new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented,
        //        NullValueHandling = NullValueHandling.Ignore
        //    });

        //    return result;
        //}
        // 07. Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            /*Get all categories. Order them in descending order by the category’s products count.
             For each category select its name, the number of products, the average price of those products (rounded to second digit after the decimal separator)
             and the total revenue (total price sum and rounded to second digit after the decimal separator) of those products (regardless if they have a buyer or not).*/

            var categories = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .ProjectTo<CategoriesByProductsCountDTO>()
                .ToList();

            var result = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return result; // not matching again
        }
        // 06. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            /*Get all users who have at least 1 sold item with a buyer.
             Order them by last name, then by first name.
             Select the person's first and last name.
             For each of the sold products (products with buyers), select the product's name, price and the buyer's first and last name.*/
            var userProducts = context
                .Users
                .Where(u => u.ProductsSold.Any(ps =>
                    ps.Buyer != null)) // check if user has sold at least one product to a buyer
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ProjectTo<GetUsersWithSoldProducts>()
                .ToList();

            string result = JsonConvert.SerializeObject(userProducts, Formatting.Indented);

            return result; // not matching word doc 
        }
        // 05. Export Products in Range with Auto-mapper and DTO
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context // still some seller names are null
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<GetProductsInRangeDTO>()
                .ToList();

            string result = JsonConvert.SerializeObject(products, Formatting.Indented);

            return result;
        }
        // 05. Export Products in Range
        //public static string GetProductsInRange(ProductShopContext context)
        //{
        //    /*Get all products in a specified price range:  500 to 1000 (inclusive).
        //     Order them by price (from lowest to highest).
        //     Select only the product name, price and the full name of the seller.
        //     Export the result to JSON.*/
        //    var products = context // for some reason seller name sometimes is null
        //        .Products
        //        .Where(x => x.Price >= 500 && x.Price <= 1000)
        //        .Select(x => new
        //        { 
        //            name = x.Name,
        //            price = x.Price,
        //            seller = x.SellerFullName.FirstName + " " + x.SellerFullName.LastName
        //        })
        //        .OrderBy(x => x.price)
        //        .ToList();

        //    string result = JsonConvert.SerializeObject(products, Formatting.Indented);

        //    return result;
        //}
        // 04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            //Import the users from the provided file categories-products.json. 
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }
        // 03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            /*Import the users from the provided file categories.json.
             Some of the names will be null, so you don’t have to add them in the database.
             Just skip the record and continue.*/
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(c => c.Name != null)
                .ToList();

            context.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        // 02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            //Import the users from the provided file products.json.
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
        // 01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            //Import the users from the provided file users.json.
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);

            context.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
        private static void ResetDataBase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was deleted!");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was created!");
        }
        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
        }
    }
}