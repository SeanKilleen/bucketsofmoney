using BoDi;
using Marten;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using Weasel.Core;

namespace BucketsOfMoney.Domain.Tests;

[Binding]
public class MartenSupport
{
    private IDocumentStore _documentStore;
    private readonly IObjectContainer _objectContainer;
    private readonly ISpecFlowOutputHelper _outputHelper;

    public MartenSupport(IObjectContainer objectContainer, ISpecFlowOutputHelper outputHelper)
    {
        _objectContainer = objectContainer;
        _outputHelper = outputHelper;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _documentStore = DocumentStore.For(_ =>
        {
            _.Connection($"host=localhost;database={TestContainerSupport.DatabaseName};password={TestContainerSupport.Password};username={TestContainerSupport.Username};Include Error Detail=true");

            _.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

            _.Events.AddEventType<AccountCreated>();
            _.Events.AddEventType<BucketCreated>();
            _.Events.AddEventType<PoolFundsTransferredIntoBucket>();
        });
        
        _objectContainer.RegisterInstanceAs(_documentStore);
    }

    [AfterScenario()]
    public async Task AfterScenario()
    {
        await _documentStore.Advanced.Clean.DeleteAllEventDataAsync();
    }
}