using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<ImportUsersDto, User>();

            this.CreateMap<ImportProductsDto, Product>();

            this.CreateMap<ImportCategoriesDto, Category>();

            this.CreateMap<Product, ExportProductsInRangeDto>()
                .ForMember(x => x.BuyerName, y => y.MapFrom(s => s.Buyer.FirstName + " " + s.Buyer.LastName))
                .ForMember(x => x.Price, y => y.MapFrom(z => z.Price))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name));

        }
    }
}
