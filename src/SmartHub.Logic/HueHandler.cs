using System.Net.Http;

namespace SmartHub.Logic
{
    public class HueHandler : HttpClientHandler
    {

        public HueHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual;
            ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
        }

    }
}
