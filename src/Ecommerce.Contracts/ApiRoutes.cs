namespace Ecommerce.Contracts;

public static class ApiRoutes
{
    public const string Root = "/api";
    
    public static class Auth
    {
        public const string Register = Root + "/auth/register";
        public const string ConfirmEmail = Root + "/auth/confirm-email";
        public const string RegisterAdmin = Root + "/auth/register-admin";
        public const string Login = Root + "/auth/login";
        public const string Logout = Root + "/auth/logout"; 
    }

    public static class Basket
    {
        public const string AddProduct = Root + "/basket/add-product";
        public const string IncreaseProduct = Root + "/basket/increase-product";
        public const string DecreaseProduct = Root + "/basket/decrease-product";
        public const string RemoveProduct = Root + "/basket/remove-product";
        public const string GetProducts = Root + "/basket/get-products";
        public const string GetProductIds = Root + "/basket/get-product-ids";
    }

    public static class Brand
    {
        public const string GetAll = Root + "/brand";
        public const string GetById = Root + "/brand/{id}";
        public const string Create = Root + "/brand";
        public const string Edit = Root + "/brand/{id}";
        public const string Delete = Root + "/brand{id}";
    }

    public static class Category
    {
        public const string GetAll = Root + "/category";
        public const string GetById = Root + "/category/{id}";
        public const string Create = Root + "/category";
        public const string Edit = Root + "/category/{id}";
        public const string Delete = Root + "/category/{id}";
    }

    public static class Payment
    {
        public const string Pay = Root + "/payment/pay";
        public const string Refound = Root + "/payment/refund";
    }

    public static class Product
    {
        public const string GetAll = Root + "/product";
        public const string GetById = Root + "/product/{id}";
        public const string Create = Root + "/product";
        public const string Edit = Root + "/product/{id}";
        public const string Delete = Root + "/product/{id}";
    }

    public static class Store
    {
        public const string Edit = Root + "/store/{id}";
        public const string Delete = Root + "/store/{id}";
        public const string GetAll = Root + "/store";
        public const string GetById = Root + "/store/{id}";
        public const string Create = Root + "/store";
        public const string IncreaseProduct = Root + "/store/increase-product/{id}";
        public const string DecreaseProduct = Root + "/store/decrease-product/{id}";
        public const string GetStoreWithProduct = Root + "/store/get-with-product/{id}";
        public const string GetStoreProductsPaginated = Root + "/store/get-with-product-paginated";  
    }

    public static class Token
    {
        public const string Refresh = Root + "/token/refresh";
        public const string Revoke = Root + "/token/revoke"; 
    }
}