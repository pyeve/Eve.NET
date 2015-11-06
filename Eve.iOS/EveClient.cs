using System.Net.Http;

namespace Eve.iOS
{
    class EveClient : Eve.EveClient
    {
        protected override HttpClient GetHttpClient()
        {
            return new HttpClient(new ModernHttpClient.NativeMessageHandler());
        }
    }
}
