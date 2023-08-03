using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using ToDoList.DAL.DbContext;
using ToDoListApp.DAL;

namespace ToDoListAppTest.Fixtures
{
    public class DockerWebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _dbContainer;
        public int InitialUsersCount { get; } = 3;

        public DockerWebApplicationFactoryFixture()
        {
            _dbContainer = new PostgreSqlBuilder().Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connectionString = _dbContainer.GetConnectionString();
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<UsersDbContext>));
                services.AddDbContext<UsersDbContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });
            });

            Console.Write($"Postgressql: {connectionString}");
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            using (var scope = Services.CreateScope())
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

        public async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
