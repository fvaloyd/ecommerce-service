using AutoMapper;
using Ecommerce.Api.Dtos.Category;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.APi.Dtos.Brand;
using Ecommerce.Core.Entities;

namespace Ecommerce.Api;

public class ApplicationMappings : Profile
{
    public ApplicationMappings()
    {
        CreateMap<Product, GetProductDto>();
        CreateMap<PostProductDto, Product>();
        CreateMap<PutProductDto, Product>();

        CreateMap<PostStoreDto, Store>();
        CreateMap<PutStoreDto, Store>();
        CreateMap<IEnumerable<ProductStore>, StoreWithProductDto>()
            .ForMember(dest => dest.ProductsName, opt => opt.MapFrom(src => src.Select(ps => ps.Product.Name)));

        CreateMap<PostBrandDto, Brand>();
        CreateMap<PutBrandDto, Brand>();

        CreateMap<PostCategoryDto, Category>();
        CreateMap<PutCategoryDto, Category>();
    }
}
