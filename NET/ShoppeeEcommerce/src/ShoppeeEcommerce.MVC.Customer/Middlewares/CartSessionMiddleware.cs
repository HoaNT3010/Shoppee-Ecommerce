namespace ShoppeeEcommerce.MVC.Customer.Middlewares
{
    public class CartSessionMiddleware
    {
        const string CookieName = "cart_session";
        readonly RequestDelegate _next;
        public CartSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if ((!context.User.Identity?.IsAuthenticated ?? true) && !context.Request.Cookies.ContainsKey(CookieName))
            {
                context.Response.Cookies.Append(CookieName,
                    Guid.NewGuid().ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Path = "/",
                        Expires = DateTime.UtcNow.AddYears(1)
                    });
            }
            await _next(context);
        }
    }
}
