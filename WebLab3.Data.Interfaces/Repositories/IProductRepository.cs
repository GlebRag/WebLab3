using WebLab3.Data.Interfaces.Models;

namespace WebLab3.Data.Interfaces.Repositories
{
    public interface IProductRepository<T> : IBaseRepository<T>
        where T : IProductData
    {
    }
}
