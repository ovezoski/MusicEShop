using MusicEShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Repository.Interface
{
    public interface IOrderRepository 
    {
        List<Order> GetAllOrders();
        Order GetDetailsForOrder(BaseEntity id);
        List<Order> GetOrdersByUserId(string userId);
    }
}
