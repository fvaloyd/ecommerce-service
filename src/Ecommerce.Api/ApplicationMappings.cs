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
        CreateMap<Product, ProductResponse>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<EditProductRequest, Product>();

        CreateMap<CreateStoreRequest, Store>();
        CreateMap<EditStoreRequest, Store>();
        CreateMap<(IEnumerable<ProductResponse>, Store), StoreResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Item1))
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Item2));

        CreateMap<CreateBrandRequest, Brand>();
        CreateMap<EditBrandRequest, Brand>();

        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<EditCategoryRequest, Category>();
    }
}
