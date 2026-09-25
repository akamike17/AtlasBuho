using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Translation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// User Secrets are only loaded by convention when the app is launched via `dotnet run` or
// from the CLI with the assembly-level UserSecretsId; this project disables that attribute
// (GenerateAssemblyInfo=false), so add the store explicitly in Development.
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets("AtlasBuho");
}

builder.Services.AddControllersWithViews()
    // 6B.md §21/§22: enums are serialized by name so the API contract stays stable and readable.
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// 6B.md §3: the translator core resolves ITranslationEngine; no direct vendor dependency.
// The canonical dataset lives in the Phase-1 verified MySQL database.
var connectionString = builder.Configuration.GetConnectionString("MySQL");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AtlasBuhoDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    builder.Services.AddScoped<ITranslationEngine, DictionaryTranslationEngine>();
}

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

app.UseAuthorization();

app.MapControllers(); // [ApiController] attribute routes: /api/translate
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
