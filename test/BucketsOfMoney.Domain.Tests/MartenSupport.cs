using BoDi;
using Marten;
using TechTalk.SpecFlow;

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

    [BeforeScenario()]
    public void BeforeScenario()
    {
        _documentStore = DocumentStore.For($"host=localhost;database={TestContainerSupport.DatabaseName};password={TestContainerSupport.Password};username={TestContainerSupport.Username}");
        _objectContainer.RegisterInstanceAs(_documentStore);
    }
}