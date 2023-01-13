using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dlw.Integration.DataAccelerator.Functions.Domain.Interfaces.IOC;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.IOC
{
    /// <summary>
    /// Represents a singleton container builder.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Method which is run at function startup that instantiates container singletons by deserializing appsettings. 
        /// </summary>
        /// <param name="builder">The <see cref="IFunctionsHostBuilder"/>.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            var type = configuration.GetValue<string>("IOCContainer:Type") ?? "Blob";
            switch (type)
            {
                case "Share":
                    //builder.Services.AddSingleton<IIOCContainer>(c => configuration.GetSection("IOCContainer").Get<ShareIOCContainer>());
                    builder.Services.AddSingleton<IIOCContainer>(c => {
                        var shareIOCContainer = new ShareIOCContainer();
                        c.GetRequiredService<IConfiguration>().GetSection("IOCContainer").Bind(shareIOCContainer);
                        return shareIOCContainer;
                    });
                    break;
                default:
                    //builder.Services.AddSingleton<IIOCContainer>(c => configuration.GetSection("IOCContainer").Get<BlobIOCContainer>());
                    builder.Services.AddSingleton<IIOCContainer>(c => {
                        var blobIOCContainer = new BlobIOCContainer();
                        c.GetRequiredService<IConfiguration>().GetSection("IOCContainer").Bind(blobIOCContainer);
                        return blobIOCContainer;
                    });
                    break;
            }
        }
    }
}