using TUnit.Aspire;

namespace ocent.DevEnvironment.AcceptanceTests.TestProject;

public class AppFixture : AspireFixture<Projects.ocent_DevEnvironment_AppHost>
{
    protected override void ConfigureBuilder(IDistributedApplicationTestingBuilder builder)
    {
        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
    }
}