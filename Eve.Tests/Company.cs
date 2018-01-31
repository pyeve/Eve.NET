using Newtonsoft.Json;

namespace Eve.Tests
{
    /// <summary>
    /// This test class inherits from BaseClass (providing meta-field mappings)
    /// and also remaps its fields to Json properties. In both Json and MongoDB
    /// it is quite common to adopt short field names.
    /// </summary>
    public class Company : BaseClass
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        /// <value>The company name.</value>
        public string Name { get; set; }
        public string Password { get; set; }
        public string StateOrProvince { get; set; }
    }
}
