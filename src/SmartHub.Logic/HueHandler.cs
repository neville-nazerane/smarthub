using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class HueHandler : HttpClientHandler
    {

        public HueHandler()
        {
            //MaxConnectionsPerServer = 1;
            ClientCertificateOptions = ClientCertificateOption.Manual;
            ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version = new Version(2, 0);
            return base.SendAsync(request, cancellationToken);
        }

    }
}
