using map_tile_server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Extension
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddLogger(builder.Host);
// Configure Database
builder.Services.ConfigureMongoDatabaseSettings(builder.Configuration);
builder.Services.ConfigureMapTileServer(builder.Configuration);
builder.Services.ConfigureOsmDatabaseSettings(builder.Configuration);
builder.Services.ConfigureEmailSettings(builder.Configuration);
// Add services to the container.
builder.Services.ConfigureServices();
builder.Services.AddControllers();
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
