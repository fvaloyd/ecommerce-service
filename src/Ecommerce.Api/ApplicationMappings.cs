using Ecommerce.Api.Dtos.Category;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.APi.Dtos.Brand;
using Ecommerce.Core.Entities;

using AutoMapper;
using Ecommerce.Api.Dtos.Brand;
using Ecommerce.Application.Common.Models;

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
