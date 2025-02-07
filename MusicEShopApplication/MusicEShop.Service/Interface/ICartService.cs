using MusicEShop.Domain.DomainModels;
using MusicEShop.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Service.Interface
{
    public interface ICartService
    {
        CartDTO getShoppingCartInfo(string userId);
        bool deleteItemFromShoppingCart(string userId, Guid trackId, Guid albumId);
        bool order(string userId);
        bool AddToShoppingConfirmed(CartItem model, string userId);
    }
}
