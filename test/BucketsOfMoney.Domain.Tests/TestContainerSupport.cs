using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.Hosting;
using TestContainers.Container.Database.PostgreSql;

namespace BucketsOfMoney.Domain.Tests
{
    [Binding]
    public static class TestContainerSupport
    {
        private static bool ContainerHasBeenInitialized = false;

        public const string Username = "testuser";
        public const string Password = "testpw";
        public const string DatabaseName = "bucketsofmoney";

        [BeforeTestRun]
        public static void SetupTestContainer()
        {
            if (!ContainerHasBeenInitialized)
            {
                ContainerHasBeenInitialized = true;
                var container = new ContainerBuilder<PostgreSqlContainer>()
                    .ConfigureDatabaseConfiguration("testuser", "testpw", "bucketsofmoney")
                    .ConfigureLogging(builder =>
                    {
                        builder.AddConsole();
                    })
                    .Build();

                container.StartAsync().Wait();
            }
        }
    }
}
