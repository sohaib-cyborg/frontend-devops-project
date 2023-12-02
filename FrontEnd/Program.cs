using FrontEnd.Services;
using FrontEnd.Services.Address;
using FrontEnd.Services.CustomerPayments;
using FrontEnd.Services.Orders;
using FrontEnd.Services.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<IAddressServices, AddressServices>();
builder.Services.AddScoped<ICustomerPayment,CustomerPayment>();
builder.Services.AddScoped<IProductServices,ProductServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
