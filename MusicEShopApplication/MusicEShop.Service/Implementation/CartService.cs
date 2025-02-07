using MusicEShop.Domain.DomainModels;
using MusicEShop.Domain.DTO;
using MusicEShop.Repository.Interface;
using MusicEShop.Service.Interface;
using MusicEShop.Domain;
using MusicEShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Service.Implementation
{
    public class CartService : ICartService

    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IEmailService _emailService;

        public CartService(IRepository<Cart> cartRepository, IRepository<CartItem> cartItemRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository, IEmailService emailService)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _emailService = emailService;
        }

        public bool AddToShoppingConfirmed(CartItem model, string userId)
        {
            var loggedInUser = _userRepository.Get(userId);
            var userCart = loggedInUser.Cart;

            if (userCart.CartItems == null)
                userCart.CartItems = new List<CartItem>(); ;

            userCart.CartItems.Add(model);
            _cartRepository.Update(userCart);
            return true;
        }

        public bool deleteItemFromShoppingCart(string userId, Guid trackId, Guid albumId)
        {
            var user = _userRepository.Get(userId);
            if (user == null || user.Cart == null)
                return false;

            var cart = _cartRepository.GetById(user.Cart.Id);
            if (cart == null || cart.CartItems == null)
                return false;

            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ( ci.TrackId == trackId) ||
                                      ( ci.AlbumId == albumId));

            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                _cartItemRepository.Delete(cartItem); 
                _cartRepository.Update(cart);
                return true;
            }

            return false;
        }

        public CartDTO getShoppingCartInfo(string userId)
        {
            var loggedInUser = _userRepository.Get(userId);

            var userCart = loggedInUser?.Cart;
            var allItems = userCart?.CartItems?.ToList();

            var totalPrice = allItems?.Select(x =>
        ((x.Track != null ? x.Track.Price : x.Album?.Price) ?? 0) * x.Quantity).Sum() ?? 0;

            CartDTO dto = new CartDTO
            {
                CartItem = allItems?.Select(x => new CartItemDTO
                {
                    ItemId = x.Id,
                    Name = x.Track != null ? x.Track.Title : x.Album?.Title ?? "Unknown Item",
                    Quantity = x.Quantity,
                    Price = (x.Track != null ? x.Track.Price : x.Album?.Price) ?? 0
                }).ToList() ?? new List<CartItemDTO>(),
                TotalPrice = totalPrice
            };
            return dto;
        }

        public bool order(string userId)
        {
            if (userId != null)
            {
                var loggedInUser = _userRepository.Get(userId);

                var userCart = loggedInUser?.Cart;
                if (userCart == null || userCart.CartItems == null || !userCart.CartItems.Any())
                    return false; 

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser
                };

                _orderRepository.Insert(order);

                List<OrderItem> orderItems = new List<OrderItem>();

                var orderedItems = userCart.CartItems.Select(x => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Order = order,
                    TrackId = x.Track?.Id,
                    Track = x.Track,
                    AlbumId = x.Album?.Id,
                    Album = x.Album,
                    Quantity = x.Quantity,
                    Price = (x.Track != null ? x.Track.Price : x.Album?.Price) ?? 0
                }).ToList();

                EmailMessage message = new EmailMessage
                {
                    Subject = "Successful Order",
                    MailTo = loggedInUser.Email
                };

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Your order is completed. The order contains:");

                decimal totalPrice = 0;

                for (int i = 0; i < orderedItems.Count; i++)
                {
                    var item = orderedItems[i];
                    string itemName = item.Track != null ? item.Track.Title : item.Album?.Title ?? "Unknown Item";
                    decimal itemPrice = item.Price;
                    totalPrice += itemPrice * item.Quantity;

                    sb.AppendLine($"{i + 1}. {itemName} - Quantity: {item.Quantity}, Price: ${itemPrice}");
                }

                sb.AppendLine($"Total price for your order: ${totalPrice}");
                message.Content = sb.ToString();

                orderItems.AddRange(orderedItems);
                foreach (var orderItem in orderItems)
                {
                    _orderItemRepository.Insert(orderItem);
                }

                userCart.CartItems.Clear();
                _cartRepository.Update(userCart);

                _emailService.SendEmailAsync(message);

                return true;
            }
            return false;
        }
    }
}
