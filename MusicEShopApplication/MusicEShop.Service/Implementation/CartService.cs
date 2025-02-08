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
using Stripe.Checkout;

namespace MusicEShop.Service.Implementation
{
    public class CartService : ICartService

    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Domain.DomainModels.OrderItem> _orderItemRepository;
        private readonly IEmailService _emailService;
        private IPaymentService _paymentService;

        public CartService(IRepository<Cart> cartRepository, IRepository<CartItem> cartItemRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<Domain.DomainModels.OrderItem> orderItemRepository, IEmailService emailService, IPaymentService paymentService)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _emailService = emailService;
            _paymentService = paymentService;   
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

        public bool DeleteItemFromShoppingCart(string userId, Guid albumId)
        {
            var user = _userRepository.Get(userId);
            if (user?.Cart?.Id == null)
                return false;

            var cart = _cartRepository.GetById(user.Cart.Id);
            if (cart?.CartItems == null)
                return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.AlbumId == albumId);
            if (cartItem == null)
                return false;

            cart.CartItems.Remove(cartItem);
            _cartItemRepository.Delete(cartItem); // Assuming this updates the DB

            // Optional: If needed, update the cart in DB
            _cartRepository.Update(cart);

            return true;
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
                CartItem = allItems?.Select(x => new Domain.DomainModels.OrderItem
                {
                    OrderId = Guid.NewGuid(),
                    TrackId = x.Track?.Id,
                    AlbumId = x.Album?.Id,
                    Track = x.Track,
                    Album = x.Album,
                    Quantity = x.Quantity,
                    Price = (x.Track != null ? x.Track.Price : x.Album?.Price) ?? 0
                }).ToList() ?? [],
                TotalPrice = totalPrice
            };
            return dto;

        }

        public SessionCreateOptions order(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var loggedInUser = _userRepository.Get(userId);
            if (loggedInUser?.Cart?.CartItems?.Any(x => x.Album != null) != true)
                return null;

            Order order = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = loggedInUser
            };

            _orderRepository.Insert(order);

            List<Domain.DomainModels.OrderItem> orderItems = [];

            var orderedItems = loggedInUser.Cart.CartItems
                .Where(x => x.Album != null)  // Only include albums
                .Select(x => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Order = order,
                    AlbumId = x.Album.Id,
                    Album = x.Album,
                    Quantity = x.Quantity,
                    Price = x.Album.Price
                })
                .ToList();

            if (!orderedItems.Any()) // Just an extra safety check
                return null;

            EmailMessage message = new EmailMessage
            {
                Subject = "Successful Order",
                MailTo = loggedInUser.Email
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your order is completed. The order contains:");

            decimal totalPrice = 0;
            var sessionOptions = _paymentService.CreateSessionOptions(orderedItems);

            for (int i = 0; i < orderedItems.Count; i++)
            {
                var item = orderedItems[i];
                sb.AppendLine($"{i + 1}. {item.Album.Title} - Quantity: {item.Quantity}, Price: ${item.Price}");
                totalPrice += item.Price * item.Quantity;
            }

            sb.AppendLine($"Total price for your order: ${totalPrice}");
            message.Content = sb.ToString();

            orderItems.AddRange(orderedItems);
            foreach (var orderItem in orderItems)
            {
                _orderItemRepository.Insert(orderItem);
            }
            loggedInUser.Cart.CartItems.Clear();
            _cartRepository.Update(loggedInUser.Cart);
            
            _emailService.SendEmailAsync(message);

            return sessionOptions;
        }

    }
}
