using Newtonsoft.Json;

namespace Eve.Tests
{
    public class Company : BaseClass
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        /// <value>The company name.</value>
        [JsonProperty("n")]
        public string Name { get; set; }
        [JsonProperty("p")]
        public string Password { get; set; }
    }
}
