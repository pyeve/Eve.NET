using System.Net.Http.Headers;

namespace Eve
{
    public interface IAuthenticator
    {
        AuthenticationHeaderValue AuthenticationHeader();
    }
}
