using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductsEntity?> GetProductById(int Id);
        Task<IList<ProductsEntity>> GetProductByIds(List<int> Ids);
        Task<IEnumerable<ProductsEntity>> GetProducts();
        Task<ProductsEntity> AddProducts(ProductsEntity entity);
        Task<ProductsEntity> UpdateProducts(int ProductId, ProductsEntity entity);
        Task<bool> DeleteProducts(int ProductId);
    }
}
