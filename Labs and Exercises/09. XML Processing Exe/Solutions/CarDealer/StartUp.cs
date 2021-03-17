using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Export.GetCarsWithTheirListOfParts;
using CarDealer.DTO.Export.SalesWithAppliedDiscount;
using CarDealer.DTO.Import;
using CarDealer.Models;
using CarDealer.XMLHelper;

namespace CarDealer
{
    public class StartUp
    {
        private const string DatasetsDirectoryPath = "../../../Datasets";

        private const string ResultsDirectoryPath = "../../../Datasets/Results";
        public static void ResetDb(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database deleted successfully");
            context.Database.EnsureCreated();
            Console.WriteLine("Database created successfully");
        }
        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<CarDealerProfile>(); });
        }
        public static void Main(string[] args)
        {
            using CarDealerContext context = new CarDealerContext();

            InitializeMapper();

            //ResetDb(context);

            //// 9. Import Suppliers
            //string inputXml = File.ReadAllText($"{DatasetsDirectoryPath}/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, inputXml));

            //// 10. Import Parts
            //string inputXml2 = File.ReadAllText($"{DatasetsDirectoryPath}/parts.xml");
            //Console.WriteLine(ImportParts(context, inputXml2));

            //// 11. Import Cars
            //string inputXml3 = File.ReadAllText($"{DatasetsDirectoryPath}/cars.xml");
            //Console.WriteLine(ImportCars(context, inputXml3));

            //// 12. Import Customers
            //string inputXml4 = File.ReadAllText($"{DatasetsDirectoryPath}/customers.xml");
            //Console.WriteLine(ImportCustomers(context, inputXml4));

            //// 13. Import Sales
            //string inputXml5 = File.ReadAllText($"{DatasetsDirectoryPath}/sales.xml");
            //Console.WriteLine(ImportSales(context, inputXml5));


            // 14.Cars With Distance
            var xml1 = GetCarsWithDistance(context);
            File.WriteAllText($"{ResultsDirectoryPath}/cars.xml", xml1);

            // 15.Cars from make BMW
            var xml2 = GetCarsFromMakeBmw(context);
            File.WriteAllText($"{ResultsDirectoryPath}/bmw-cars.xml", xml2);

            // 16.Local Suppliers
            var xml3 = GetLocalSuppliers(context);
            File.WriteAllText($"{ResultsDirectoryPath}/local-suppliers.xml", xml3);

            // 17.Cars with Their List of Parts
            var xml4 = GetCarsWithTheirListOfParts(context);
            File.WriteAllText($"{ResultsDirectoryPath}/cars-and-parts.xml", xml4);

            // 18.Total Sales by Customer
            var xml5 = GetTotalSalesByCustomer(context);
            File.WriteAllText($"{ResultsDirectoryPath}/customers-total-sales.xml", xml5);

            // 19.Sales with Applied Discount
            var xml6 = GetSalesWithAppliedDiscount(context);
            File.WriteAllText($"{ResultsDirectoryPath}/sales-discounts.xml", xml6);

        }
        // 19.Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //Get all sales with information about the car, customer and price of the sale with and without discount.
            var result = context
                .Sales
                .Select(s => new SalesWithAppliedDiscountDto
                {
                    Car = new CarDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price) - (s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100)) // not sure if to format?
                })
                .ToList();

            var xml = XmlConverter.Serialize(result, "sales");

            return xml;
        }
        // 18.Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            /*Get all customers that have bought at least 1 car and get their names, bought cars count and total spent money on cars.
             Order the result list by total spent money descending.*/
            var result = context
                .Customers
                .Where(c => c.Sales.Any())
                .Select(c => new TotalSalesByCustomerDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales
                        .Select( s=>s.Car)
                        .SelectMany(cars=>cars.PartCars)
                        .Sum(pc=>pc.Part.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToList();

            var xml = XmlConverter.Serialize(result, "customers");

            return xml;
        }
        // 17.Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            /*Get all cars along with their list of parts. For the car get only make, model and travelled distance and for the parts get only name and price and sort all parts by price (descending).
             Sort all cars by travelled distance (descending) then by model (ascending).
             Select top 5 records.*/
            var result = context
                .Cars
                .Select(c => new CarsWithTheirPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new PartsDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                        .OrderByDescending(pc => pc.Price)
                        .ToList()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToList();

            var xml = XmlConverter.Serialize(result, "cars");

            return xml;
        }
        // 16. Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            /*Get all suppliers that do not import parts from abroad.
             Get their id, name and the number of parts they can offer to supply. */
            var result = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new GetLocalSuppliersDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var xml = XmlConverter.Serialize(result, "suppliers");

            return xml;
        }
        // 15.Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            //Get all cars from make BMW and order them by model alphabetically and by travelled distance descending.
            var result = context
                .Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new CarsWithMakeDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToList();

            var xml = XmlConverter.Serialize(result, "cars");

            return xml;
        }
        //14. Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            /*Get all cars with distance more than 2,000,000.
            Order them by make, then by model alphabetically. Take top 10 records.*/
            var result = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new CarsWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToList();
            string xml = XmlConverter.Serialize(result, "cars");
            return xml;
        }
        // 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            //Import the sales from the provided file sales.xml. If car doesn’t exists, skip whole entity.

            var sales = XmlConverter.Deserializer<ImportSalesDto>(inputXml, "Sales")
                .Where(s => context.Cars.Any(c => c.Id == s.CarId))
                .Select(s => new Sale()
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }
        // 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            //Import the customers from the provided file customers.xml.
            var customerResults = XmlConverter.Deserializer<ImportCustomersDto>(inputXml, "Customers");

            var customers = customerResults
                .Select(c => new Customer
                {
                    Name = c.Name,
                    BirthDate = DateTime.Parse(c.BirthDate), // not sure if string or DateTime
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }
        // 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            //Import the cars from the provided file cars.xml. Select unique car part ids. If the part id doesn’t exists, skip the part record.
            var carsDtos = XmlConverter.Deserializer<ImportCarsDto>(inputXml, "Cars");

            var cars = new List<Car>();

            foreach (var car in carsDtos)
            {
                var partsIds = car.Parts
                    .Select(p => p.Id)
                    .Distinct()
                    .Where(id => context.Parts.Any(p => p.Id == id))
                    .ToList();

                var currCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance,
                    PartCars = partsIds.Select(id => new PartCar()
                    {
                        PartId = id
                    })
                        .ToList()
                };
                cars.Add(currCar);
            }

            context.Cars.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }
        // 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            //Import the parts from the provided file parts.xml. If the supplierId doesn’t exists, skip the record.
            var partsResult = XmlConverter.Deserializer<ImportPartsDto>(inputXml, "Parts");

            var parts = partsResult
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                .Select(p => new Part
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SupplierId = p.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }
        // 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            //Import the suppliers from the provided file suppliers.xml. 
            var supplierResult = XmlConverter.Deserializer<ImportSuppliersDto>(inputXml, "Suppliers");

            var suppliers = supplierResult
                .Select(s => new Supplier
                {
                    Name = s.Name,
                    IsImporter = s.IsImporter
                })
                .ToList();

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }
    }
}