using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ISaleService, SaleService>();

builder.Services.AddDbContext<PosContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PosContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
