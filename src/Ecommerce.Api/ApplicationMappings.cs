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
        CreateMap<Product, ProductResponse>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<EditProductRequest, Product>();

        CreateMap<CreateStoreRequest, Store>();
        CreateMap<EditStoreRequest, Store>();
        CreateMap<(IEnumerable<ProductStore>, Store), StoreResponse>()
            .ForMember(dest => dest.ProductsName, opt => opt.MapFrom(src => src.Item1.Select(ps => ps.Product.Name)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Item2.Name))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Item2.State));

        CreateMap<CreateBrandRequest, Brand>();
        CreateMap<EditBrandRequest, Brand>();

        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<EditCategoryRequest, Category>();
    }
}
