using System.Net;
using Flurl.Http;

namespace Tixtrack.WebApiInterview.EndToEndTests.Controllers.Base;

internal static class FlurlResponseStatus
{
    public static HttpStatusCode Status(this IFlurlResponse response) =>
        (HttpStatusCode)response.StatusCode;
}