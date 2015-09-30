using System.Net.Http.Headers;

namespace Eve
{
	public class BearerAuthenticator : IAuthenticator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BearerAuthenticator"/> class.
		/// </summary>
		/// <param name="token">Access token.</param>
		public BearerAuthenticator (string token)
		{
		    Token = token;
		}

		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		/// <value>The access token.</value>
		public string Token { get; set; }

		/// <summary>
		/// Returns an encoded authentication header which can be used to perform authenticated requests.
		/// </summary>
		/// <returns>The encoded authentication header.</returns>
		public AuthenticationHeaderValue AuthenticationHeader ()
		{
		    return new AuthenticationHeaderValue("Bearer", Token);
		}
	}
}

