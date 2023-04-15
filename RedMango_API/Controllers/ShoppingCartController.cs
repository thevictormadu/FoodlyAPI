using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Services;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
       
        public ShoppingCartController(ApplicationDbContext db, IBlobService blobService)
        {
            _db = db;
            _response = new ApiResponse();


           
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantity)
        {
            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(u => u.CartItems).FirstOrDefault(u => u.UserId== userId);
            var menuItems = _db.MenuItems.FirstOrDefault(u => u.Id== menuItemId);

            if (menuItems== null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess= false;
                return BadRequest(_response);
            }

            if (shoppingCart == null && updateQuantity > 0)
            {
                ShoppingCart newCart = new() { UserId = userId };
                _db.ShoppingCarts.Add(newCart);
                _db.SaveChanges();


                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQuantity,
                    ShoppingCartId = newCart.Id,
                    MenuItem = null
                };

                _db.CartItems.Add(newCartItem);
                _db.SaveChanges();
            }
            else
            {
                //if shopping cart exists

                CartItem cartIteminCart = shoppingCart.CartItems.FirstOrDefault(u => u.MenuItemId== menuItemId);
                if (cartIteminCart == null)
                {
                    //item does not exist in current cart
                    var newCartItem = new CartItem()
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantity,
                        ShoppingCartId = shoppingCart.Id,
                        MenuItem = null

                    };
                    _db.CartItems.Add(newCartItem); _db.SaveChanges();
                }
                else
                {
                    //item exist in the cart and we have to update quantity
                    int newQuantity = cartIteminCart.Quantity + updateQuantity; 
                    if (newQuantity <= 0) 
                    { 
                        _db.CartItems.Remove(cartIteminCart);
                        if (shoppingCart.CartItems.Count() == 1)
                        {
                            _db.ShoppingCarts.Remove(shoppingCart);
                        }
                        _db.SaveChanges();
                    }

                    else
                    {
                        cartIteminCart.Quantity = newQuantity;
                        _db.SaveChanges();
                    }

                }

            }

            return _response;
        }
    }
}
