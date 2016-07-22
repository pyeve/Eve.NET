using System;
using System.Net.Http.Headers;

namespace Eve.Authenticators
{
	public class BasicAuthenticator : IAuthenticator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BasicAuthenticator"/> class.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		public BasicAuthenticator (string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password { get; set; }

		/// <summary>
		/// Returns an encoded authentication header which can be used to perform authenticated requests.
		/// </summary>
		/// <returns>The encoded authentication header.</returns>
		public AuthenticationHeaderValue AuthenticationHeader ()
		{
			var s = string.Format ("{0}:{1}", UserName, Password);
			return new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (ToAscii (s)));
		}

		/// <summary>
		/// Converts a string to an ASCII-encoded byte array.
		/// </summary>
		/// <returns>The ASCII-encoded byte array.</returns>
		/// <param name="s">S.</param>
		private static byte[] ToAscii (string s)
		{
			var r = new byte[s.Length];
			for (var ix = 0; ix < s.Length; ++ix) {
				var ch = s [ix];
				if (ch <= 0x7f)
					r [ix] = (byte)ch;
				else
					r [ix] = (byte)'?';
			}
			return r;
		}
	}
}

