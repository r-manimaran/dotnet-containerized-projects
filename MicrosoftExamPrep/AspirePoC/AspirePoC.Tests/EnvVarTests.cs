using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aspire.Hosting;

namespace AspirePoC.Tests
{
    public class EnvVarTests
    {
        [Fact]
        public async Task WebResourceEnvVarsResolveToApiService()
        {
            // Arrange
            var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspirePoC_AppHost>();

            //var frontend = builder.CreateResourceBuilder<ProjectResource>("webfrontend");

            // Act
            //var envVars = await frontend.Resource.GetEnvironmentVariableValuesAsync(Aspire.Hosting.DistributedApplicationOperation.Publish);

            ////Assert
            //Assert.Contains(envVars, static (kvp) =>
            //{
            //    var (key, value) = kvp;

            //    return key is "services__apiservice__https__0" &&
            //           value is "{apiservice.bindings.https.url}";
            //});

        }
    }
}
