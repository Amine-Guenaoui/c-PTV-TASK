
using StreetService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
          options.SerializerSettings.Converters.Add(
            new NetTopologySuite.IO.Converters.GeometryConverter()); // Ensure correct converter is used0
    });;

builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection"));


//swagger 
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI in the app
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Enables the Swagger API documentation
    app.UseSwaggerUI();  // Enables the Swagger UI for interactive API testing
}

app.MapControllers();
app.Run();
