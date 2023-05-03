using BoDi;
using Marten;
using Org.BouncyCastle.Crypto.Fpe;
using TechTalk.SpecFlow;
using Weasel.Core;

namespace BucketsOfMoney.Domain.Tests;

[Binding]
public class MartenSupport
{
    private IDocumentStore _documentStore;
    private readonly IObjectContainer _objectContainer;

    public MartenSupport(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _documentStore = DocumentStore.For(_ =>
        {
            _.Connection($"host=localhost;database={TestContainerSupport.DatabaseName};password={TestContainerSupport.Password};username={TestContainerSupport.Username};Include Error Detail=true");
            _.AutoCreateSchemaObjects = AutoCreate.All;
            _.Events.AddEventType<AccountCreated>();
            _.Events.AddEventType<BucketCreated>();
        });

        _objectContainer.RegisterInstanceAs(_documentStore);
    }

    [AfterScenario()]
    public async Task AfterScenario()
    {
        await _documentStore.Advanced.Clean.DeleteAllEventDataAsync();
    }
}