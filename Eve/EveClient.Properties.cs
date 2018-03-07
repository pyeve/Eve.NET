using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eve.Authenticators;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Eve
{
    public partial class EveClient : IDisposable
    {
        /// <summary>
        /// Gets or sets the remote service base address.
        /// </summary>
        /// <value>The remote service base address.</value>
        public Uri BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource endpoint.
        /// </summary>
        /// <value>The name of the resource endpoint.</value>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>The document identifier.</value>
        /// <remarks>Used in conjuction with BaseAddress and ResourceName to construct the document endpoint.</remarks>
        public string DocumentId { get; set; }

        /// <summary>
        /// Represents a HTTP response message.
        /// </summary>
        /// <value>The http response.</value>
        public HttpResponseMessage HttpResponse { get; private set; }

        /// <summary>
        /// Gets or sets the authenticator.
        /// </summary>
        /// <value>The authenticator.</value>
        public IAuthenticator Authenticator { get; set; }

        /// <summary>
        /// Gets or sets the name of the LastUpdated field.
        /// </summary>
        public string LastUpdatedField { get; set; }

        /// <summary>
        /// Gets or sets the name of the Deleted field.
        /// </summary>
        public string DeletedField { get; set; }
        /// <summary>
        /// Gets or set the custom headers to be included with the request.
        /// </summary>
        public Dictionary<string, string> CustomHeaders { get; set; }
    }
}
