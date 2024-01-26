using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using SztukaNaWidoku.Database;
using SztukaNaWidoku.Database.Repositories;
using SztukaNaWidoku.Filters;
using SztukaNaWidoku.Jobs;
using SztukaNaWidoku.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite("Data Source=Database.db"));
builder.Services.AddScoped<ExhibitionRepository>();
builder.Services.AddScoped<ScrappingMNWService>();
builder.Services.AddScoped<ScrappingMSNService>();
builder.Services.AddScoped<ScrappingPGSService>();
builder.Services.AddScoped<ScrappingUjazdowskiService>();
builder.Services.AddScoped<ScrappingZachetaService>();
builder.Services.AddScoped<ScrappingCSWLazniaService>();
builder.Services.AddScoped<ScrappingMMGService>();
builder.Services.AddHangfire(a => a.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage());
builder.Services.AddHangfireServer();
builder.Services.AddScoped<GetExhibitionsDataJob>();
builder.Services.AddScoped<DeleteAllExhibitionsDataJob>();
builder.Services.AddSwaggerGen();
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(opts =>
{
    opts.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    }
});

app.UseGetExhibitionsDataJob();
app.UseDeleteAllExhibitionsDataJob();

app.Run();

