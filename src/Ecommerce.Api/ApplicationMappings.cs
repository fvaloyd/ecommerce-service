using Ecommerce.Core.Entities;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

using AutoMapper;

namespace Ecommerce.Api;

public class ApplicationMappings : Profile
{
    public ApplicationMappings()
    {
        CreateMap<CreateProductRequest, Product>();
        CreateMap<EditProductRequest, Product>();
        CreateMap<Product, ProductResponse>();

        CreateMap<CreateStoreRequest, Store>();
        CreateMap<EditStoreRequest, Store>();
        CreateMap<Store, StoreResponse>();
        CreateMap<(IEnumerable<ProductResponse>, Store), StoreWithProductResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Item1))
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Item2));

        CreateMap<CreateBrandRequest, Brand>();
        CreateMap<EditBrandRequest, Brand>();
        CreateMap<Brand, BrandResponse>();

        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<EditCategoryRequest, Category>();
        CreateMap<Category, CategoryResponse>();
    }
}
