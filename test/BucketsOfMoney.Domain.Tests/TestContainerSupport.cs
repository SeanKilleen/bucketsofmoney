using BoDi;
using Marten;
using Microsoft.Extensions.Logging;
using TechTalk.SpecFlow;
using TestContainers.Container.Abstractions.Hosting;
using TestContainers.Container.Database.Hosting;
using TestContainers.Container.Database.PostgreSql;

namespace BucketsOfMoney.Domain.Tests
{
    [Binding]
    public class MartenSupport
    {
        private IDocumentStore _documentStore;
        private readonly IObjectContainer _objectContainer;

        public MartenSupport(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario()]
        public void BeforeScenario()
        {
            _documentStore = DocumentStore.For($"host=localhost;database={TestContainerSupport.DatabaseName};password={TestContainerSupport.Password};username={TestContainerSupport.Username}");
            _objectContainer.RegisterInstanceAs(_documentStore);
        }
    }

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
