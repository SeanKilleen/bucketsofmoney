using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;
using Testcontainers.PostgreSql;

namespace BucketsOfMoney.Domain.Tests
{
    [Binding]
    public static class TestContainerSupport
    {
        private static bool ContainerHasBeenInitialized = false;

        public const string Username = "testuser";
        public const string Password = "testpw";
        public const string DatabaseName = "bucketsofmoney";

        public static PostgreSqlContainer _postgreSqlContainer;

        [BeforeTestRun]
        public static void SetupTestContainer()
        {
            if (!ContainerHasBeenInitialized)
            {
                ContainerHasBeenInitialized = true;

                _postgreSqlContainer= new PostgreSqlBuilder()
                    .WithUsername(Username)
                    .WithPassword(Password)
                    .WithDatabase(DatabaseName)
                    .WithAutoRemove(true)
                    .Build();
                
                _postgreSqlContainer.StartAsync().Wait();
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _postgreSqlContainer.StopAsync().Wait();
        }
    }
}
