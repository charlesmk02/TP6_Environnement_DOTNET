using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Configuration CORS pour autoriser le client Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs.API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT généré par AuthentificationApi"
    });

    
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

// Configuration JWT (Keycloak)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"] ?? "http://localhost:8180/realms/GestionOrdinateursRealm";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

// Injection des dépendances pour le service d'ordinateurs et le contexte de la base de données
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();

// Injection du contexte de la base de données pour l'accès aux données
builder.Services.AddDbContext<ParcInformatiqueContext>();

// service de réparation
builder.Services.AddScoped<IReparationService, ReparationService>();

// service de stock
builder.Services.AddSingleton<IStockService, StockService>();

// journalisation
builder.Services.AddSingleton<IJournalService, JournalService>();

var app = builder.Build();

//Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();
    
    // TP 5.3
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login = "user1", Role = RoleEnum.Admin },
            new User { Login = "user2", Role = RoleEnum.Gestionnaire },
            new User { Login = "user3", Role = RoleEnum.Gestionnaire }
        );
        db.SaveChanges();
    }
}

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
