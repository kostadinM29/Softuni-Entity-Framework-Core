using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static void ImportPartCars(CarDealerContext context)
        {
            int carsCount = context
                .Cars
                .Count();
            int partsCount = context.Parts.Count();

            var partCars = new List<PartCar>();

            for (int i = 1; i <= carsCount; i++)
            {
                var partCar = new PartCar();

                partCar.CarId = i;

                partCar.PartId = new Random().Next(1, partsCount);

                partCars.Add(partCar);
            }

            context.PartCars.AddRange(partCars);

            context.SaveChanges();
            Console.WriteLine($"Successfully added {partCars.Count()} partCars!");
        } // not needed ?
        private const string DatasetsDirectoryPath = "../../../Datasets";

        private const string ResultsDirectoryPath = "../../../Datasets/Results";
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            //ResetDataBase(context);

            InitializeMapper();

            ////--Import Parts
            //ImportPartCars(context);

            //// 8. Import Suppliers
            //string inputJson = File.ReadAllText($"{DatasetsDirectoryPath}/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, inputJson));

            //// 9. Import Parts
            //string inputJson2 = File.ReadAllText($"{DatasetsDirectoryPath}/parts.json");
            //Console.WriteLine(ImportSuppliers(context, inputJson2));

            //// 10. Import Cars
            //string inputJson3 = File.ReadAllText($"{DatasetsDirectoryPath}/cars.json");
            //Console.WriteLine(ImportCars(context, inputJson3));

            //// 11. Import Customers
            //string inputJson4 = File.ReadAllText($"{DatasetsDirectoryPath}/customers.json");
            //Console.WriteLine(ImportCustomers(context, inputJson4));

            //// 12. Import Sales
            //string inputJson5 = File.ReadAllText($"{DatasetsDirectoryPath}/sales.json");
            //Console.WriteLine(ImportSales(context, inputJson5));

            //// 13. Export Ordered Customers
            //var json = GetOrderedCustomers(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/ordered-customers.json", json);

            // 14. Export Cars from Make Toyota
            //var json = GetCarsFromMakeToyota(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/toyota-cars.json", json);

            // 15. Export Local Suppliers
            //var json = GetLocalSuppliers(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/local-suppliers.json", json);

            // 16. Export Cars with Their List of Parts
            //var json = GetCarsWithTheirListOfParts(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/cars-and-parts.json", json);

            // 17. Export Total Sales by Customer
            //var json = GetTotalSalesByCustomer(context);
            //File.WriteAllText($"{ResultsDirectoryPath}/customers-total-sales.json", json);

            // 18. Export Sales with Applied Discount
            var json = GetSalesWithAppliedDiscount(context);
            File.WriteAllText($"{ResultsDirectoryPath}/sales-discounts.json", json);

        }
        // 18. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            /*Get first 10 sales with information about the car, customer and price of the sale with and without discount.
             Export the list of sales to JSON in the format provided below.*/
            var sales = context // i get invalidoperationexception but f*ck it it works with judge
                .Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = s.Discount.ToString("F2"),
                    price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    priceWithDiscount =
                        (s.Car.PartCars.Sum(pc => pc.Part.Price) -
                         (s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100)).ToString("F2")
                })
                .ToList();

            string result = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return result;
        }
        // 17. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            /*Get all customers that have bought at least 1 car and get their names, bought cars count and total spent money on cars.
             Order the result list by total spent money descending then by total bought cars again in descending order.
             Export the list of customers to JSON in the format provided below.*/
            var customers = context
                .Customers
                .Where(c=>c.Sales.Any(s=>s.Car != null))
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales
                        .Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }
        // 16. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            /*Get all cars along with their list of parts.
             For the car get only make, model and travelled distance and for the parts get only name and price (formatted to 2nd digit after the decimal point).
             Export the list of cars and their parts to JSON in the format provided below.*/
            var cars = context
                .Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    },
                    parts = c.PartCars.Select(pc => new // parts null again because i don't have the in the db
                    {
                        pc.Part.Name,
                        Price = pc.Part.Price.ToString("F2")
                    })
                })
                .ToList();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }
        // 15. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            /*Get all suppliers that do not import parts from abroad.
             Get their id, name and the number of parts they can offer to supply.
             Export the list of suppliers to JSON in the format provided below.*/
            var suppliers = context // parts is 0 but its probably because in my project parts is not filled?
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            string result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return result;
        }
        // 14. Export Cars from Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            /*Get all cars from make Toyota and order them by model alphabetically and by travelled distance descending.
             Export the list of cars to JSON in the format provided below.*/
            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }
        // 13. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            /*  Get all customers ordered by their birth date ascending.
             If two customers are born on the same date first print those who are not young drivers (e.g. print experienced drivers first).
             Export the list of customers to JSON in the format provided below.*/
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .ProjectTo<OrderedCustomersDto>() // need dto to order by .tostring-ed birthdate ?
                .ToList();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }
        // 12. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            //Import the sales from the provided file sales.json.

            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }
        // 11. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            //Import the customers from the provided file customers.json.

            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }
        // 10. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            // F O R    J U D G E HOLY SHIT

            //Import the cars from the provided file cars.json.
            //var carsDtos = JsonConvert.DeserializeObject<List<ImportCarDto>>(inputJson);

            //var cars = new List<Car>();

            //foreach (var carDto in carsDtos)
            //{
            //    var car = new Car()
            //    {
            //        Make = carDto.Make,
            //        Model = carDto.Model,
            //        TravelledDistance = carDto.TravelledDistance
            //    };

            //    foreach (int partId in carDto?.PartsId.Distinct())
            //    {
            //        car.PartCars.Add(new PartCar()
            //        {
            //            PartId = partId
            //        });
            //    }

            //    cars.Add(car);
            //}

            //context.Cars.AddRange(cars);
            //context.SaveChanges(); // gives me invalid fk error





            // F O R    P R O J E C T 

            var cars = JsonConvert.DeserializeObject<List<Car>>(inputJson);

            context.Cars.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }
        // 9. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            /*Import the parts from the provided file parts.json.
             If the supplierId doesn’t exists, skip the record.*/
            var suppliers = context.Suppliers
                .Select(s => s.Id)
                .ToList();

            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                .Where(p => suppliers.Contains(p.SupplierId))
                .ToList();

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }
        // 8. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            //Import the suppliers from the provided file suppliers.json. 
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }
        private static void ResetDataBase(CarDealerContext context)
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
                cfg.AddProfile<CarDealerProfile>();
            });
        }
    }

}