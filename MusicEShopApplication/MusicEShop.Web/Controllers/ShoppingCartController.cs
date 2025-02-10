using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using MusicEShop.Service.Interface;
using Newtonsoft.Json;

namespace MusicEShop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ICartService _shoppingCartService;
        private readonly IOrderService _orderService;
            public ShoppingCartController(ICartService shoppingCartService,IOrderService orderService)
        {
            this._shoppingCartService = shoppingCartService;
            this._orderService = orderService;
        }

        public IActionResult Index()
        {
            var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dto=_shoppingCartService.getShoppingCartInfo(userId);
            return View(dto);
        }

        public IActionResult DeleteFromShoppingCart(Guid itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = _shoppingCartService.DeleteItemFromShoppingCart(userId,itemId);
            // if(result == 0) -> throw 
            return RedirectToAction("Index", "ShoppingCart");

        }

        public IActionResult Order()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var options = _shoppingCartService.order(userId);
            if (options == null)
                throw new NullReferenceException();
            TempData["CheckoutOptions"] = JsonConvert.SerializeObject(options);
            return RedirectToAction("Checkout", "Payment");

        }


    }
}
