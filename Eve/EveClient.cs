using Eve.Authenticators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve
{
    public partial class EveClient : IDisposable
	{
        public EveClient ()
		{
			// don't serialize null values.
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings { 
				NullValueHandling = NullValueHandling.Ignore,
			};

			LastUpdatedField = "_updated";
		    DeletedField = "_deleted";
            CustomHeaders = new Dictionary<string, string>();
            ContractResolver = new EveContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
		}

		public EveClient (Uri baseAddress) : this ()
		{
			BaseAddress = baseAddress;
		}


		public EveClient (string baseAddress) : this (new Uri (baseAddress))
		{
		}

		public EveClient (string baseAddress, IAuthenticator authenticator) : this (baseAddress)
		{
			Authenticator = authenticator;
		}

		public EveClient (Uri baseAddress, IAuthenticator authenticator) : this (baseAddress)
		{
			Authenticator = authenticator;
		}
        
	    private async Task<List<T>> ParseJsonAsListOf<T>(HttpContent content)
	    {
			var json = await content.ReadAsStringAsync ();
			var jo = JObject.Parse (json);
            var settings = new JsonSerializerSettings() { ContractResolver = ContractResolver };
			return JsonConvert.DeserializeObject<List<T>> (jo.Property ("_items").Value.ToString (Formatting.None), settings);
	    }

	    public void Dispose() { }
        /// <summary>
        /// Gets or sets the contract resolver.
        /// </summary>
        /// <value>
        /// The contract resolver.
        /// </value>
        public static EveContractResolver ContractResolver { get; set; }
	}
}
