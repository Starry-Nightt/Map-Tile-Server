using map_tile_server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.ConfigureMongoDatabaseSettings(builder.Configuration);
builder.Services.ConfigureMapTileServer(builder.Configuration);
builder.Services.AddLogger(builder.Host);
builder.Services.ConfigureServices();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseConfigureCustomExceptionMiddleware();

app.UseConfiguredCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
