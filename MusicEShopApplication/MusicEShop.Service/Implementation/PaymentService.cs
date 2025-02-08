using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicEShop.Domain.DomainModels;
using MusicEShop.Service.Interface;
using Stripe.Checkout;

namespace MusicEShop.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        public SessionCreateOptions CreateSessionOptions(List<OrderItem> orderedItems)
        {
            return new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = orderedItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Convert to cents
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Album.Title
                        },
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment"
            };
        }
    }
}
