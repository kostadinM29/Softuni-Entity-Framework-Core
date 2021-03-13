using System.Linq;
using AutoMapper;
using ProductShop.DTO;
using ProductShop.DTO.GetUsersWithProducts;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, GetProductsInRangeDTO>()
                .ForMember(x => x.SellerFullName, y => y.MapFrom(s => s.Seller.FirstName + " " + s.Seller.LastName));

            this.CreateMap<Product, GetSoldProduct>()
                .ForMember(x => x.BuyerFirstName, y => y.MapFrom(x => x.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, y => y.MapFrom(x => x.Buyer.LastName));

            this.CreateMap<User, GetUsersWithSoldProducts>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold.Where(p => p.Buyer != null)));

            this.CreateMap<Category, CategoriesByProductsCountDTO>()
                .ForMember(x => x.Category, y => y.MapFrom(s => s.Name))
                .ForMember(x => x.ProductsCount, y => y.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product).Count())) // if this wasn't many to many maybe it was possible to skip this?
                .ForMember(x => x.AveragePrice, y => y.MapFrom(s => s.CategoryProducts.Average(cp => cp.Product.Price).ToString("F2")))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(s => s.CategoryProducts.Sum(cp => cp.Product.Price).ToString("F2")));

            //this.CreateMap<User, GetAllUsersDTO>();

            //this.CreateMap<User, GetUsersWithProductsDTO>()
            //    .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold.Where(p => p.Buyer != null)));

            //this.CreateMap<User, GetSoldProductsDTO>()
            //    .ForMember(x => x.Count, y => y.MapFrom(u => u.ProductsSold.Count(p => p.Buyer != null)))
            //    .ForMember(x => x.Products, y => y.MapFrom(u => u.ProductsSold.Where(p => p.Buyer != null)));

            //this.CreateMap<Product, GetProductsDTO>(); // not needed to .formember i think?



        }
        public class AutoMapperConfiguration
        {
            public static void Configure()
            {
                Mapper.Initialize(x => x.AddProfile<ProductShopProfile>());

                Mapper.Configuration.AssertConfigurationIsValid();
            }
        }
    }
}
