using LibraryAPI.Caching;
using LibraryAPI.Caching.Interfaces;
using LibraryAPI.Contexts;
using LibraryAPI.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSqlServer<LibraryContext>(connectionString);

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICustomCache, InMemoryCache>();

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = connectionString;
    options.SchemaName = "dbo";
    options.TableName = "Cache";
});
builder.Services.AddScoped<ICustomCache, DistributedCache>();

//builder.Services.AddStackExchangeRedisCache((options) =>
//{
//    var redisSettings = builder.Configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
//    options.Configuration = $"{redisSettings.Host}:{redisSettings.Port}";
//});

//builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
//builder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<RedisSettings>>().Value);
//builder.Services.AddSingleton<RedisConnector>();

//builder.Services.AddScoped<ICustomCache, RedisCache>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
