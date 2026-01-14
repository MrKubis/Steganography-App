using Stenography.API;

var builder = WebApplication.CreateBuilder(args);

string? secKey = Environment.GetEnvironmentVariable("AES_KEY");

if (string.IsNullOrEmpty(secKey))
{
    Console.WriteLine("Environment variable AES_KEY is not set");
    return;
}

builder.Services.AddSingleton(new CryptoService(secKey));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAngularDev");

app.MapControllers();

app.Run();
