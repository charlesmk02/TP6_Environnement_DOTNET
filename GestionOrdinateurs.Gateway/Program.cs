var builder = WebApplication.CreateBuilder(args);

// Configuration des CORS pour autoriser le client Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// YARP Reverse Proxy configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowBlazorClient");

// Activation du Reverse Proxy YARP
app.MapReverseProxy();

app.Run();