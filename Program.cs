using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
// Add services to the container.

//var secretKey = builder.Configuration.GetValue<string>("JwtSetting:SecretKey");
//services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(item =>
//{
//    item.RequireHttpsMetadata = false;
//    item.SaveToken = true;
//    item.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ClockSkew = TimeSpan.Zero
//    };
//});



services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

//services.AddAzureClients(clientBuilder =>
//{
//    clientBuilder.AddServiceBusClient(configuration["ServiceBus:ConnectionString"]);
//});


// Policy-Based Authorization. For more complex scenarios (e.g., Admins can delete, Managers can approve), define policies
// Since we already issue multiple roles in the token, ASP.NET Core automatically checks them against the [Authorize(Policy = "...")] attribute.
services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUser", policy => policy.RequireRole("User"))
    .AddPolicy("RequireAdminOrManager", policy => policy.RequireRole("Admin", "Manager"))
    .AddPolicy("HRDepartment", policy => policy.RequireClaim("Department", "HR"));

services.AddHttpClient("orders").AddServiceDiscovery();

services.AddRateLimiter(options =>
    {
        // Rate limiting, once 5 requests are hit in 10 seconds, the 6th request is rejected.
        options.AddFixedWindowLimiter("rateLimit", opt =>
        {
            opt.PermitLimit = 5;              // max 5 requests
            opt.Window = TimeSpan.FromSeconds(10);
            opt.QueueLimit = 0;               // no queue → reject immediately
        });

        // Throttling, the 6th request is queued and processed later, as long as the queue isn’t full.
        options.AddFixedWindowLimiter("throttle", opt =>
        {
            opt.PermitLimit = 5;              // process 5 requests
            opt.Window = TimeSpan.FromSeconds(10);
            opt.QueueLimit = 20;              // queue up to 20 requests
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });

        // Fixed window policy
        options.AddFixedWindowLimiter(policyName: "fixed", op =>
        {
            op.QueueLimit = 2; // allow 2 queued requests
            op.PermitLimit = 5; // max 5 requests
            op.Window = TimeSpan.FromSeconds(10); // per 10 seconds
            op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Sliding window policy
        options.AddSlidingWindowLimiter("sliding", opt =>
        {
            opt.PermitLimit = 10;
            opt.Window = TimeSpan.FromSeconds(30);
            opt.SegmentsPerWindow = 3;
            opt.QueueProcessingOrder = QueueProcessingOrder.NewestFirst;
            opt.QueueLimit = 5;
        });

        // Token bucket policy
        options.AddTokenBucketLimiter("token", opt =>
        {
            opt.TokenLimit = 20;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 5;
            opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
            opt.TokensPerPeriod = 5;
            opt.AutoReplenishment = true;
        });
    });
        
WebApplication app = builder.Build();

app.UseRateLimiter(); // Enable rate limiting globally

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
         .UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(config =>
    config.AllowAnyOrigin()
             .AllowAnyHeader()
             .AllowAnyMethod()
);

app.Run();
