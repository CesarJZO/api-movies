using ControllersAPI;
using ControllersAPI.ApiBehavior;
using ControllersAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddCors(options =>
{
    // string[]? frontendUrls = builder.Configuration.GetSection("Frontend").GetValue<string[]>("Urls");
    // var frontendUrl = builder.Configuration.GetValue<string>("Frontend:Url");
    var urls = builder.Configuration.GetSection("Frontend:Urls").Get<string[]>();

    options.AddDefaultPolicy(builder =>
        builder.WithOrigins(urls!)
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<BadRequestParser>();
}).ConfigureApiBehaviorOptions(BadRequestsBehavior.Parse);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
