using Ecommerce.Api.Dtos.Category;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.APi.Dtos.Brand;
using Ecommerce.Core.Entities;

using AutoMapper;

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
        CreateMap<(IEnumerable<ProductStore>, Store), StoreWithProductDto>()
            .ForMember(dest => dest.ProductsName, opt => opt.MapFrom(src => src.Item1.Select(ps => ps.Product.Name)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Item2.Name))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Item2.State));

        CreateMap<CreateBrandRequest, Brand>();
        CreateMap<EditBrandRequest, Brand>();

        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<EditCategoryRequest, Category>();
    }
}
