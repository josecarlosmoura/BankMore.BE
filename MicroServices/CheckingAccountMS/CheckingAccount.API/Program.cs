using Application.Services.CheckingAccount;
using BuildingBlocks.Auth;
using BuildingBlocks.Auth.Interfaces;
using BuildingBlocks.Settings;
using CheckingAccountMS.Application.Commands.AuthenticateUser;
using CheckingAccountMS.Application.Commands.CreateAccount;
using CheckingAccountMS.Application.Commands.CreateTransaction;
using CheckingAccountMS.Application.Commands.CreateTransfer;
using CheckingAccountMS.Application.Commands.DeactivateAccount;
using CheckingAccountMS.Application.Queries.GetAccountBalance;
using CheckingAccountMS.Infrastructure.Data;
using CheckingAccountMS.Infrastructure.Repository.Implementation;
using CheckingAccountMS.Infrastructure.Repository.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankMore.BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<ICheckingAccountService, CheckingAccountServiceImpl>();

            // TODO: Melhorar a organização dos repositórios - extrair para arquivo separado
            builder.Services.AddScoped<ICheckingAccountRepository, CheckingAccountRepositoryImpl>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepositoryImpl>();
            builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepositoryImpl>();

            // TODO: Melhorar a organização dos comandos e queries - extrair para arquivo separado
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthenticateUserCommand).Assembly));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DeactivateAccountCommand).Assembly));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTransactionCommand).Assembly));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTransferCommand).Assembly));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAccountBalanceQuery).Assembly));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
