using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.DAL.DbContext;
using ToDoListApp.DAL;

namespace ToDoListAppTest.Fixtures
{
    public class WebApplicationFactoryFixture : IAsyncLifetime 
    {
        // вынести в appsettings
        //private const string _connectionString = "Server=localhost; Port=5432; Database=ToDoList;User id=postgres;password=1111;";

        private WebApplicationFactory<Program> _factory;

        public HttpClient Client { get; private set; }
        public int InitialUsersCount { get; set; } = 3;

        public WebApplicationFactoryFixture()
        {
            var configuration = GetConfiguration();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(Services =>
                {
                    Services.RemoveAll(typeof(DbContextOptions<UsersDbContext>));
                    Services.AddDbContext<UsersDbContext>(options =>
                    {
                        options.UseNpgsql(connectionString);
                    });
                });
            });
            Client = _factory.CreateClient();
        }

        public IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var cntx = scopedServices.GetRequiredService<UsersDbContext>();

                await cntx.Database.EnsureDeletedAsync();
            }
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var cntx = scopedServices.GetRequiredService<UsersDbContext>();

                await cntx.Database.EnsureCreatedAsync();

                var seedManager = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seedManager.SeedDataAsync();

                var users = DataFixture.GetUsers(InitialUsersCount);
                await seedManager.CreateUsers(users, ToDoListApp.Enums.Roles.User);

                await cntx.SaveChangesAsync();
            }
        }
    }
}
