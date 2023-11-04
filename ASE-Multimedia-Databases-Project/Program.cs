using ASE_Multimedia_Databases_Project.Contexts;
using ASE_Multimedia_Databases_Project.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "cors-policy",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:44463");
                      });
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IPokemonDbContext, PokemonDbContext>();
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
