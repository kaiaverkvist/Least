using System.Text.Json.Serialization;
using Least;
using Least.ExampleAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// So that the user mapping can take place.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// This is required because the resource does not know exactly what type of DbContext you are using.
builder.Services.AddScoped<DbContext, ExampleDbContext>();
builder.Services.AddDbContext<ExampleDbContext>(opt => opt.UseInMemoryDatabase("ExampleDB"));
var app = builder.Build();

new Bootstrapper<ExampleDbContext>(app);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExampleDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "Hello World!");

app.Run();