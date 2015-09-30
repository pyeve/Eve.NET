using System.Net.Http.Headers;

namespace Eve.Authenticators
{
    public interface IAuthenticator
    {
        AuthenticationHeaderValue AuthenticationHeader();
    }
}
