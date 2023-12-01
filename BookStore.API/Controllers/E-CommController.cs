using BookStore.API.Data;
using BookStore.API.Model;
using BookStore.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class E_CommController : ControllerBase
    {
        private readonly BookStoreContext _context;
        public E_CommController(BookStoreContext context)
        {
            _context = context;
        }


        //SEED DATA
        [HttpPost("seedData")]
        public IActionResult SeedData()
        {
            
            var adminRole = new role { Id = Guid.NewGuid(), Name = "admin" };
            var sellerRole = new role { Id = Guid.NewGuid(), Name = "seller" };
            var userRole = new role { Id = Guid.NewGuid(), Name = "user" };

            _context.Role.AddRange(adminRole, sellerRole, userRole);

        
            var user1 = new user
            {
                Id = Guid.NewGuid(),
                UserName = "sampleuser1",
                UserEmail = "user1@example.com",
                UserPassword = "samplepassword1",
                UserPhone = "1234567890",
                ActivityStatus = "active"
            };

            var user2 = new user
            {
                Id = Guid.NewGuid(),
                UserName = "sampleuser2",
                UserEmail = "user2@example.com",
                UserPassword = "samplepassword2",
                UserPhone = "1234567890",
                ActivityStatus = "active"
            };

            var user3 = new user
            {
                Id = Guid.NewGuid(),
                UserName = "sampleuser3",
                UserEmail = "user3@example.com",
                UserPassword = "samplepassword3",
                UserPhone = "1234567890",
                ActivityStatus = "active"
            };

            _context.Users.AddRange(user1, user2, user3);

            var userRole1 = new userRole { User = user1, Role = adminRole };
            var userRole2 = new userRole { User = user2, Role = sellerRole };
            var userRole3 = new userRole { User = user3, Role = userRole };

            _context.UserRole.AddRange(userRole1, userRole2, userRole3);

            _context.SaveChanges();

            return Ok("Sample data seeded successfully.");
        }


        //USER AUTHENTICATION
        [HttpPost("signup")]
        public IActionResult Signup(UserSignupRequest userData)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.UserEmail == userData.Email);
                if (existingUser != null)
                {
                    return BadRequest("User with this email already exists.");
                }

                var newUser = new user
                {
                    Id = Guid.NewGuid(),
                    UserName = userData.Name,
                    UserEmail = userData.Email,
                    UserPassword = userData.Password,
                    UserPhone = userData.UserPhone,
                    ActivityStatus = "active"
                };

                var role = _context.Role.FirstOrDefault(r => r.Name == userData.Role);
                if (role == null)
                {
                    return BadRequest("Invalid role.");
                }

                var userRole = new userRole
                {
                    UId = newUser.Id,
                    RId = role.Id
                };

                newUser.UserRoles = new List<userRole> { userRole };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var userResponse = new UserWithRoles
                {
                    Id = newUser.Id,
                    UserName = newUser.UserName,
                    UserEmail = newUser.UserEmail,
                    UserPassword = newUser.UserPassword,
                    UserPhone = newUser.UserPhone,
                    Role = role.Name 
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginRequest loginRequest)
        {
            var user = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.UserEmail == loginRequest.Email && u.UserPassword == loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(); 
            }

            if (user.ActivityStatus != "active")
            {
                return Forbid(); 
            }

            var userResponse = new UserWithRoles
            {
                Id = user.Id,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPassword = user.UserPassword,
                UserPhone = user.UserPhone,
                Role = user.UserRoles.FirstOrDefault()?.Role.Name
            };
            return Ok(userResponse);
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<user>>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        [HttpGet("usersWithRoles")]
        public IActionResult GetUsersWithRoles()
        {
            // Query the database to retrieve users and their roles
            var usersWithRoles = _context.Users
                .Select(user => new UserWithRoles
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    UserPassword = user.UserPassword,
                    UserPhone = user.UserPhone,
                    //Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                    Role = user.UserRoles.FirstOrDefault().Role.Name,
                    ActivityStatus =user.ActivityStatus
                })
                .ToList();

            return Ok(usersWithRoles);
        }



        //[HttpGet("{id}")]
        //public async Task<ActionResult<user>> GetUser(Guid id)
        //{
        //    var user = await _context.Users.FindAsync(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        //[HttpPost]
        //public async Task<ActionResult<user>> PostUser(user user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}

        //admin operations: add seller 
        [HttpPost("addSeller")]
        public async Task<IActionResult> AddSeller([FromBody] UserWithRoles signUp)
        {
            try
            {
                
                if (_context.Users.Any(u => u.UserEmail == signUp.UserEmail))
                {
                    return Conflict("Email is already registered.");
                }
                
                var user = new user
                {
                    UserName = signUp.UserName,
                    UserEmail = signUp.UserEmail,
                    //UserPassword = signUp.Password,
                    UserPhone = signUp.UserPhone,
                    ActivityStatus = "active"
                };
             
                var sellerRole = _context.Role.SingleOrDefault(r => r.Name == "seller");
                if (sellerRole != null)
                {
                    user.UserRoles = new List<userRole> { new userRole { Role = sellerRole } };
                }

                var generatedPassword = GenerateRandomPassword();
                user.UserPassword = generatedPassword;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var retUser = new UserWithRoles
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,

                    UserPassword = user.UserPassword,

                    UserPhone = user.UserPhone,
                    Role = user.UserRoles.FirstOrDefault()?.Role?.Name,
                    //ActivityStatus=user.ActivityStatus,

                };
                return Ok(retUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the seller.");
            }
        }
        private string GenerateRandomPassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }


        //change activity status of user
        //[HttpPut("updateActivityStatus/{userId}")]
        //public IActionResult UpdateActivityStatus([FromRoute] Guid userId, [FromBody] UserUpdateRequest userUpdateRequest)
        //{
        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    user.ActivityStatus = userUpdateRequest.ActivityStatus;

        //    _context.SaveChanges(); 

        //    return Ok("ActivityStatus updated successfully.");
        //}
        [HttpPut("updateActivityStatus")]
        public IActionResult UpdateActivityStatus([FromBody] UserUpdateRequest userUpdateRequest)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userUpdateRequest.Id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Attach(user);
            user.ActivityStatus = userUpdateRequest.ActivityStatus; 

            _context.SaveChanges();

            return Ok("ActivityStatus updated successfully.");
        }


        // PRODUCTS

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            //var products = await _context.Products.ToListAsync();

            //return Ok(products);

            var products = await _context.Products
                .Include(p => p.ProductImages).Select(p => new
                {
                    p.id,
                    p.name,
                    p.price,
                    p.color,
                    p.catagory,
                    p.description,
                    p.ProductImages
                }) 
                .ToListAsync();

            return Ok(products);
        }

        //from query
        [HttpGet("productsQuery")]
        public async Task<IActionResult> GetProducts([FromQuery] string q)
        {

            if (!string.IsNullOrEmpty(q))
            {
                
                var products = await _context.Products
                    .Where(p =>
                        p.name.Contains(q) ||
                        p.description.Contains(q) ||
                        p.catagory.Contains(q) ||
                        p.color.Contains(q) ||
                        p.id.ToString().Contains(q) ||
                        p.price.ToString().Contains(q)
                 

                        ).Include(p => p.ProductImages)

                    .ToListAsync();

                return Ok(products);
            }

            else
            {
                //var products = await _fullStackDbContext.Products.ToListAsync();
                //return Ok(products);
                return Ok(null);
            }
        }



        //[HttpPost("products")]
        //public async Task<IActionResult> AddProduct([FromBody] Product productRequest)
        //{
        //    productRequest.id = Guid.NewGuid();

        //    _context.Products.AddAsync(productRequest);
        //    _context.SaveChanges();


        //    return Ok(productRequest);
        //}

        [HttpPost("products")]
        public async Task<IActionResult> AddProductWithImages([FromForm] Product productRequest)
        //[FromForm]
        {
            productRequest.id = Guid.NewGuid();

            var imagePaths = new List<string>();

            foreach (var file in productRequest.ImageFiles)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine("product_images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePaths.Add(filePath);
                }
            }

            var productImages = imagePaths.Select(imagePath => new ProductImage
            {
                ImagePath = imagePath
            }).ToList();


            productRequest.ProductImages = productImages;

            _context.Products.Add(productRequest);
            await _context.SaveChangesAsync();

            return Ok(productRequest);
        }




        //[HttpPut("product/{id:Guid}")]
        //public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] Product productRequest)
        //{
        //    var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.id == id);
        //    if (existingProduct == null)
        //    {
        //        return NotFound();
        //    }
        //    //productRequest.sellerId = existingProduct.sellerId;

        //    existingProduct.name = productRequest.name;
        //    existingProduct.price = productRequest.price;
        //    existingProduct.color = productRequest.color;
        //    existingProduct.catagory = productRequest.catagory;
        //    existingProduct.description = productRequest.description;
        //    //existingProduct.imageURL = productRequest.imageURL;

        //    _context.Products.Update(existingProduct);
        //    await _context.SaveChangesAsync();

        //    return Ok(existingProduct);
        //}

    
        [HttpPut("products")]
        public async Task<IActionResult> UpdateProduct( [FromForm] Product productRequest)
        {
            
            var existingProduct = await _context.Products
                .Include(p => p.ProductImages) 
                .FirstOrDefaultAsync(p => p.id == productRequest.id);

            if (existingProduct == null)
            {
                return NotFound(); 
            }

            
            existingProduct.name = productRequest.name;
            existingProduct.price = productRequest.price;
            existingProduct.color = productRequest.color;
            existingProduct.catagory = productRequest.catagory;
            existingProduct.description = productRequest.description;


            if (productRequest.ImageFiles != null && productRequest.ImageFiles.Count > 0)
            {

                existingProduct.ProductImages.Clear();

                var imagePaths = new List<string>();

                foreach (var file in productRequest.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine("product_images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        imagePaths.Add(filePath);
                    }
                }

                var productImages = imagePaths.Select(imagePath => new ProductImage
                {
                    ImagePath = imagePath
                }).ToList();

                existingProduct.ProductImages = productImages;
            }

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return Ok(existingProduct);
        }

        [HttpGet("product/{id:Guid}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id)
        {
            //var product =
            //    await _context.Products.FirstOrDefaultAsync(x => x.id == id);
            //if (product == null)
            //{
            //    return NotFound();
            //}
            //return Ok(product);

            var product = await _context.Products
            .Include(p => p.ProductImages).Select(p => new
            {
                p.id,
                p.name,
                p.price,
                p.color,
                p.catagory,
                p.description,
                p.ProductImages
            })
            .FirstOrDefaultAsync(x => x.id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        [HttpDelete("product/{id:Guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }


        //Add Product to Cart with user Login 

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] ProductCart productCart ) { 
            if (productCart == null)
            {
                return NotFound();
            }


            //_context.ProductCart.AddAsync(productCart);
            //_context.SaveChanges();
            var existingCartItem = await _context.ProductCart.FirstOrDefaultAsync(cartItem =>
            cartItem.userId == productCart.userId &&
            cartItem.productId == productCart.productId &&
            cartItem.color == productCart.color);

            if (existingCartItem != null)
            {
                //existingCartItem.quantity += productCart.quantity;
                _context.ProductCart.Remove(existingCartItem);
                _context.ProductCart.Add(productCart);
            }
            else
            {
                
                _context.ProductCart.Add(productCart);
            }

            await _context.SaveChangesAsync();

            return Ok(productCart);
        }

        //[HttpGet("cart/{userId}")]

        //public async Task<IActionResult> GetCartList([FromRoute] Guid userId, [FromBody] ProductCart productCart) { 
        //}
        [HttpGet("cart")]
        public async Task<IActionResult> GetCartList([FromQuery] Guid userId)
        {
            try
            {
                var cartList = await _context.ProductCart.Where(c => c.userId == userId).ToListAsync();
                return Ok(cartList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        //[HttpGet("cartt")]
        //public async Task<IActionResult> GetCartList2([FromQuery] Guid userId)
        //{
        //    try
        //    {
        //        var cartList = await _context.ProductCart.Where(c => c.userId == userId).ToListAsync();

        //        var cartItems = new List<ProductCartDto>();

        //        foreach (var cartItem in cartList)
        //        {
        //            var product = await _context.Products.FirstOrDefaultAsync(p => p.id == cartItem.productId && p.color == cartItem.color);

        //            if (product != null)
        //            {
        //                var productCartDto = new ProductCartDto
        //                {
        //                    ProductId = product.id,
        //                    Color = product.color,
        //                    Quantity = cartItem.quantity,
        //                    UserId = cartItem.userId,
        //                    Name = product.name,
        //                    Price = product.price,
        //                    Category = product.catagory,
        //                    Description = product.description,
        //                    ImagePaths = product.ProductImages.Select(pi => pi.ImagePath).ToList()
        //                };

        //                cartItems.Add(productCartDto);
        //            }
        //        }

        //        return Ok(cartItems);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}

        [HttpGet("cartt")]
        public async Task<IActionResult> GetCartList2([FromQuery] Guid userId)
        {
            try
            {
                //var cartItems = await _context.ProductCart
                //    .Where(c => c.userId == userId)
                //    .ToListAsync();

                //var products = new List<Product>();
                //foreach (var cartItem in cartItems)
                //{
                //    var product = await _context.Products
                //        .Include(p => p.ProductImages) 
                //        .FirstOrDefaultAsync(p => p.id == cartItem.productId && p.color == cartItem.color);

                //    if (product != null)
                //    {
                //        products.Add(product);
                //    }
                //}

                //return Ok(products);
                var cartItems = await _context.ProductCart
               .Where(c => c.userId == userId)
               .ToListAsync();

                var products = new List<ProductCartDto>();

                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products
                        .Include(p => p.ProductImages)
                        .FirstOrDefaultAsync(p => p.id == cartItem.productId && p.color == cartItem.color);

                    if (product != null)
                    {
                       
                        var cartProduct = new ProductCartDto
                        {
                            id = product.id,
                            name = product.name,
                            price = product.price,
                            color = product.color,
                            catagory = product.catagory,
                            description = product.description,
                            quantity = cartItem.quantity,
                            //ProductImages = product.ProductImages
                            ImagePaths = product.ProductImages.Select(pi => pi.ImagePath).ToList()
                        };

                        products.Add(cartProduct);
                    }
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpDelete("cart")]
        public async Task<IActionResult> DeleteCartItem([FromQuery] Guid userId, Guid productId, string color)
        {
            try
            {
                var cartItem = await _context.ProductCart.FirstOrDefaultAsync(c => c.userId == userId && c.productId == productId && c.color == color);

                if (cartItem == null)
                {
                    return NotFound("Cart item not found");
                }

                _context.ProductCart.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Ok("Cart item deleted");
            }
            catch (Exception ex)
            {            
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("cartt")]
        //deletes cart products after order is placed 
        public async Task<IActionResult> DeleteCartItemOrderPlaced([FromQuery] Guid userId)
        {
            try
            {
                var cartItem = await _context.ProductCart.FirstOrDefaultAsync(c => c.userId == userId);

                if (cartItem == null)
                {
                    return NotFound("Cart item not found");
                }

                _context.ProductCart.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Ok("Cart item deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        //orders

        [HttpGet("order")]
        public async Task<IActionResult> GetAllOrders([FromQuery] Guid userId ) {        

            try
            {
                var orders = await _context.Orders.Where(c => c.UserId == userId).ToListAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("order")]

        //public async IActionResult PlaceOrder([FromBody] Order order)
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {

            try
            {

                var newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Name = order.Name,
                    Email = order.Email,
                    Address = order.Address,
                    Contact = order.Contact,
                    TotalPrice = order.TotalPrice,
                    UserId = order.UserId,
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Order placed successfully." });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("order")]
        public async Task<IActionResult> DeleteOrder([FromQuery] Guid orderId) {

            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(c => c.Id == orderId);

                if (order == null) {
                    return NotFound("Order Not Found");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
