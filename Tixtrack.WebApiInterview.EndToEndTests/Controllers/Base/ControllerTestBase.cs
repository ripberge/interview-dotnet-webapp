using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Configuration;

namespace Tixtrack.WebApiInterview.EndToEndTests.Controllers.Base;

public abstract class ControllerTestBase
{
    protected IConfiguration Configuration { get; set; }
    protected string BaseUrl { get; set; }
    
    protected ControllerTestBase()
    {
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        BaseUrl = Configuration.GetValue<string>("Api:BaseUrl")!;
        _configureFlurl();
    }

    private void _configureFlurl()
    {
        FlurlHttp.Configure(settings =>
        {
            settings.JsonSerializer = new SystemTextJsonSerializer();
        });
        FlurlHttp.ConfigureClient(BaseUrl, client =>
        {
            client.AllowAnyHttpStatus();
            client.Settings.HttpClientFactory = new UntrustedCertHttpClientFactory();
        });
    }
}

internal class UntrustedCertHttpClientFactory : DefaultHttpClientFactory
{
    public override HttpMessageHandler CreateMessageHandler()
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true
        };
    }
}

internal class SystemTextJsonSerializer : ISerializer
{
    private JsonSerializerOptions _options { get; }

    public SystemTextJsonSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public string Serialize(object value) => JsonSerializer.Serialize(value, _options);

    public T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _options)!;

    public T Deserialize<T>(Stream stream) =>
        JsonSerializer.Deserialize<T>(stream, _options)!;
}