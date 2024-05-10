using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TodosAPI24.Data;
using TodosAPI24.Models;

namespace TodosAPI24.Config
{
    public class TodosAPI24App
    {
        private readonly WebApplicationBuilder _builder;
        private readonly WebApplication _app;

        public TodosAPI24App(string[] args)
        {
            _builder = WebApplication.CreateBuilder(args);

            ConfigureServices();

            _app = _builder.Build();

            ConfigureApp();
        }

        private void ConfigureServices()
        {
            // Add services to the container.

            ConfigureDb();

            _builder.Services.AddControllers().AddNewtonsoftJson();

            ConfigureSwagger();

            AddAuth();
        }

        private void ConfigureSwagger()
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _builder.Services.AddEndpointsApiExplorer();
            _builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        private void ConfigureDb()
        {
            var connectionString = _builder.Configuration.GetConnectionString("MySqlConnection");
            var version = new MySqlServerVersion(new Version(8, 2, 0));

            _builder.Services.AddDbContext<TodosAPI24Context>(options => options.UseMySql(connectionString, version));
        }

        private void AddAuth()
        {
            _builder.Services.AddAuthorization();

            _builder.Services.AddIdentityApiEndpoints<CustomUser>(options =>
            {
                if (_builder.Environment.IsDevelopment())
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;
                }
            }).AddEntityFrameworkStores<TodosAPI24Context>();
        }

        private void ConfigureApp()
        {
            // Configure the HTTP request pipeline.
            if (_app.Environment.IsDevelopment())
            {
                _app.UseSwagger();
                _app.UseSwaggerUI();
            }

            _app.UseHttpsRedirection();

            _app.UseAuthorization();

            _app.MapControllers();

            _app.MapIdentityApi<CustomUser>();

            ConfigureCors();
        }

        private void ConfigureCors()
        {
            // CORS = Cross Origin Resource Sharing = Tillåt anrop från klienter på en annan domän
            _app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }

        public void Run()
        {
            _app.Run();
        }
    }
}
