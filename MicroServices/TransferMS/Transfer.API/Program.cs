using Application.Services.CheckingAccount;
using BuildingBlocks.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransferMS.Infrastructure.Repository.Interface;
using TransferMS.Application.Commands.CreateTransfer;
using TransferMS.Application.Services.CheckingAccount;
using TransferMS.Application.Services.HttpClientConnect;
using TransferMS.Infrastructure.Data;
using TransferMS.Infrastructure.Repository.Implementation;

namespace Transfer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICheckingAccountService, CheckingAccountServiceImpl>();

            builder.Services.AddScoped<ITransferRepository, TransferRepositoryImpl>();
            builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepositoryImpl>();

            builder.Services.AddScoped<ICheckingAccountHttpClient, CheckingAccountHttpClient>();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TransferCommandHandler).Assembly));

            builder.Services.AddHttpClient<ICheckingAccountHttpClient, CheckingAccountHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7136/api/"); // URL da API Conta Corrente
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            // Configura autenticação JWT
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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
