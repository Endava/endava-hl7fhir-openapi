using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services.Authorization
{
    public class AuthenticationMessageHandler : HttpClientHandler
    {
        public AuthenticationHeaderValue Authorization { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            if (Authorization != null)
            {
                request.Headers.Authorization = Authorization;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
