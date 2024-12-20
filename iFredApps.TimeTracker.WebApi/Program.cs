using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Services;
using iFredApps.TimeTracker.Data.Data;
using iFredApps.TimeTracker.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddCors(options =>
{
   options.AddDefaultPolicy(builder =>
   {
      builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
   });
});

// Configure o serviço para ler a string de conexão do banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)),
        mySqlOptions =>
        {
           mySqlOptions.EnableRetryOnFailure(
               maxRetryCount: 5,
               maxRetryDelay: TimeSpan.FromSeconds(10),
               errorNumbersToAdd: null);
        }
    )
);

// Adiciona serviços ao contêiner
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();

// Leitura da chave JWT a partir das variáveis de ambiente
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
   throw new InvalidOperationException("JWT Key is missing from environment variables.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
   options.TokenValidationParameters = new TokenValidationParameters
   {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["Jwt:Issuer"],
      ValidAudience = builder.Configuration["Jwt:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(key)
   };
});

builder.Services.AddControllers();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
   c.SwaggerDoc("v1", new OpenApiInfo
   {
      Title = "iFredApps TimeTracker",
      Version = "v1"
   });

   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
      In = ParameterLocation.Header,
      Description = "JWT Authorization header using the Bearer scheme.",
      Name = "Authorization",
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer"
   });
   c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

try
{
   var app = builder.Build();

   app.Urls.Add("http://0.0.0.0:80");

   try
   {
      using (var scope = app.Services.CreateScope())
      {
         var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

         // Aplica migrações pendentes
         dbContext.Database.Migrate();
         Console.WriteLine("Database migration applied successfully.");
      }
   }
   catch (Exception ex)
   {
      Console.WriteLine("Database migration failed: " + ex.Message);
      Console.WriteLine("Stack Trace: " + ex.StackTrace);
      if (ex.InnerException != null)
      {
         Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
      }
      throw;
   }

   // Test the database connection
   try
   {
      using (var scope = app.Services.CreateScope())
      {
         var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
         dbContext.Database.EnsureCreated();
         Console.WriteLine("Database connection successful.");
      }
   }
   catch (Exception ex)
   {
      Console.WriteLine("Database connection failed: " + ex.Message);
      Console.WriteLine("Stack Trace: " + ex.StackTrace);
      if (ex.InnerException != null)
      {
         Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
      }
      throw;
   }

   app.UseHttpsRedirection();

   app.UseAuthentication();
   app.UseAuthorization();

   // Configuração do pipeline de requisições HTTP
   //if (app.Environment.IsDevelopment())
   //{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "iFredApps TimeTracker API v1");
   });
   //}

   app.MapControllers();

   app.Run();

}
catch (Exception ex)
{
   Console.WriteLine("Unhandled exception caught: " + ex.Message);
   Console.WriteLine("Stack Trace: " + ex.StackTrace);
   if (ex.InnerException != null)
   {
      Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
   }
   throw;
}