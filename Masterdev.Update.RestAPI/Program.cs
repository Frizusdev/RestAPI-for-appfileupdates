using Masterdev.Update.RestAPI.Authorization;
using Masterdev.Update.RestAPI.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.UseInlineDefinitionsForEnums();
    x.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme {
        Description = "Api Key To Access the API",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },In = ParameterLocation.Header
    };
    var requrement = new OpenApiSecurityRequirement
    {
        {scheme, new List<string>() }
    };
    x.AddSecurityRequirement(requrement);
    var filePath = Path.Combine(AppContext.BaseDirectory, "Masterdev.Update.RestAPI.xml");
    x.IncludeXmlComments(filePath);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 4, 28))));

//builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(opt =>
//        opt.UseNpgsql(builder.Configuration.GetConnectionString(connectionString)));

builder.Services.AddSingleton(Path.Combine(Directory.GetCurrentDirectory(), "Updatezips"));

//builder.Services.AddScoped<ApiKeyFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().WithOpenApi();

app.Run();