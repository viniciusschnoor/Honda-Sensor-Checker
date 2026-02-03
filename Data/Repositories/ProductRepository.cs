using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly DbSet<Product> _dbSetProduct;
        public ProductRepository(DataContext context) : base(context)
        {
            _dbSetProduct = context.Set<Product>();
        }
    }
}