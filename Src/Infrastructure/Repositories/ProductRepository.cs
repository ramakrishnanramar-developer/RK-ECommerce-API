using Domain.Enitities;
using Domain.Enum;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository(AppDBContext appDBContext) : IProductRepository
    {
        public async Task<ProductsEntity?> GetProductById(int Id)
        {
            return await appDBContext.Products.FirstOrDefaultAsync(x => x.Id == Id);
        }
        public async Task<IList<ProductsEntity>> GetProductByIds(List<int> Ids)
        {
            return await appDBContext.Products.Where(x => Ids.Contains(x.Id)).ToListAsync();
        }
        public async Task<IEnumerable<ProductsEntity>> GetProducts()
        {
            return await appDBContext.Products.ToListAsync();
        }
        public async Task<ProductsEntity> AddProducts(ProductsEntity entity)
        {
            appDBContext.Products.Add(entity);
            await appDBContext.SaveChangesAsync();
            return entity;
        }
        public async Task<ProductsEntity> UpdateProducts(int ProductId, ProductsEntity entity)
        {
            var product = await GetProductById(ProductId);
            if (product is not null)
            {
                product.ProfessionalHighRevenuePrice = entity.ProfessionalHighRevenuePrice;
                product.ProfessionalLowRevenuePrice = entity.ProfessionalLowRevenuePrice;
                product.IndividualPrice = entity.IndividualPrice;
                product.Name = entity.Name;
                product.Type = entity.Type;
                await appDBContext.SaveChangesAsync();
                return product;
            }
            return entity;
        }
        public async Task<bool> DeleteProducts(int ProductId)
        {
            var Product = await GetProductById(ProductId);
            if (Product is not null)
            {
                appDBContext.Products.Remove(Product);
                return await appDBContext.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
