using ShoeStoreLibrary.Contexts;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ShoeStoreContext>();

builder.Services.AddSession(); // Добавляем сессии
builder.Services.AddHttpContextAccessor();

CultureInfo.DefaultThreadCurrentCulture =
    new CultureInfo("ru-RU") { NumberFormat = { NumberDecimalSeparator = "." } };

//builder.Services.AddAuthentication("MyCookieAuth")
//    .AddCookie("MyCookieAuth", config =>
//    {
//        config.Cookie.Name = "UserLogin";
//        config.LoginPath = "/Login"; // Путь для авторизации
//    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Администратор", "Менеджер"));
    options.AddPolicy("ClientPolicy", policy =>
        policy.RequireRole("Клиент"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Включаем поддержку сессий
//app.UseAuthentication();
app.UseAuthorization();

//app.MapStaticAssets();
app.MapRazorPages();
  // .WithStaticAssets();

app.Run();
