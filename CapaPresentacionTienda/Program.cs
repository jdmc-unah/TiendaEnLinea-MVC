using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


/****************-Para Activar Autenticacion-*********************/

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

/****************-Para Guardar Data de la Sesion-*********************/
builder.Services.AddDistributedMemoryCache(); // Utiliza una caché en memoria para almacenar la sesión

builder.Services.
    AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20); // Configurar el tiempo de expiración de la sesión
        options.Cookie.HttpOnly = true; // Asegura que la cookie solo sea accesible a través de HTTP
        options.Cookie.IsEssential = true;// Indica si la cookie es esencial para la aplicación }
    });


/*************************************/

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

app.UseAuthentication();

app.UseAuthorization();


app.UseSession(); // Activar el middleware de sesión


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tienda}/{action=Index}/{id?}");

app.Run();

