namespace AMD201.API.Middleware
{
    public class SupabaseAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SupabaseAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Extract user ID from JWT token
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token) as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
                    
                    if (jsonToken != null)
                    {
                        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            context.Items["UserId"] = userId;
                            context.Items["UserEmail"] = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                        }
                    }
                }
                catch
                {
                    // Invalid token, continue without authentication
                }
            }

            await _next(context);
        }
    }
}
