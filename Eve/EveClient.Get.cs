using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve
{
    public partial class EveClient : IDisposable
    {
        // TODO: DRY on GetAsync methods. Lots of them share the same code.

        /// <summary>
        /// Performs an asynchronous GET request on an arbitrary endpoint.
        /// </summary>
        /// <param name="uri">Endpoint URI.</param>
        /// <param name="etag">ETag</param>
        /// <param name="ifModifiedSince">Return only documents that changed since this date.</param>
        /// <param name="showDeleted">Wether soft deleted documents should be included or not.</param>
        /// <param name="rawQuery">Return only documents that match this query.</param>
        public async Task<HttpResponseMessage> GetAsync(string uri, string etag, DateTime? ifModifiedSince, bool showDeleted, string rawQuery)
        {

            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            ValidateBaseAddress();

            using (var client = new HttpClient())
            {
                Settings(client);

                if (etag != null)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", etag);
                }

                var q = new System.Text.StringBuilder(uri);

                // TODO don't add ims and query parts if arguments are null? Works this way, but
                // there's probably an unneeded impact on performance (and complexity).
                var imsPart =
                    ifModifiedSince == null ?
                        "{}" :
                        string.Format(@"{{""{0}"": {{""$gt"": ""{1}""}}}}", LastUpdatedField,
                            ((DateTime)ifModifiedSince).ToString("r"));

                var queryPart = @rawQuery ?? "{}";

                if (imsPart != "{}" || queryPart != "{}")
                {
                    q.Append(string.Format(@"?where={{""$and"": [{0}, {1}]}}", imsPart, queryPart));
                }

                if (showDeleted) q.Append(@"&show_deleted");

                HttpResponse = await client.GetAsync(q.ToString()).ConfigureAwait(false);
                return HttpResponse;
            }
        }

        #region Overloads

        /// <summary>
        /// Performs an asynchronous GET request on an arbitrary endpoint.
        /// </summary>
        /// <param name="uri">Endpoint URI.</param>
        /// <param name="etag">ETag</param>
        public async Task<HttpResponseMessage> GetAsync(string uri, string etag)
        {
            return await GetAsync(uri, etag, null, false, null);

        }

        /// <summary>
        /// Performs an asynchronous GET request on an arbitrary endpoint.
        /// </summary>
        /// <param name="uri">Endpoint URI.</param>
        /// <param name="ifModifiedSince">Return only documents that changed since this date.</param>
        public async Task<HttpResponseMessage> GetAsync(string uri, DateTime? ifModifiedSince)
        {
            return await GetAsync(uri, null, ifModifiedSince, false, null);
        }

        /// <summary>
        /// Performs an asynchronous GET request on an arbitrary endpoint.
        /// </summary>
        /// <param name="uri">Endpoint URI.</param>
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            return await GetAsync(uri, etag: null);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a document endpoint.
        /// </summary>
        /// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="etag">Document ETag.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<T> GetAsync<T>(string resourceName, string documentId, string etag)
        {
            ValidateResourceName(resourceName);

            if (documentId == null)
            {
                throw new ArgumentNullException("documentId");
            }

            HttpResponse = await GetAsync(string.Format("{0}/{1}", resourceName, documentId), etag);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(T);
            return await DeserializeObject<T>(HttpResponse);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a document endpoint.
        /// </summary>
        /// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<T> GetAsync<T>(string resourceName, string documentId)
        {
            return await GetAsync<T>(resourceName, documentId, null);
        }

        /// <summary>
        /// Performs an asynchronous GET request of a document.
        /// </summary>
        /// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
        /// <param name="obj">The instance of the document to be retrieved.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<T> GetAsync<T>(object obj)
        {
            ValidateResourceName();
            return await GetAsync<T>(ResourceName, obj);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a document endpoint.
        /// </summary>
        /// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="obj">The instance of the document to be retrieved.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<T> GetAsync<T>(string resourceName, object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var retObj = await GetAsync<T>(resourceName, GetDocumentId(obj), GetETag(obj));

            return HttpResponse.StatusCode == HttpStatusCode.NotModified ? (T)obj : retObj;
        }

        /// <summary>
        /// Performs an asynchronous GET request for a specific document.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>()
        {
            ValidateResourceName();
            return await GetAsync<T>(ResourceName);
        }

        /// <summary>
        /// Performs an asynchronous GET request for a specific document.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(DateTime? ifModifiedSince)
        {
            ValidateResourceName();
            return await GetAsync<T>(ResourceName, ifModifiedSince);
        }

        /// <summary>
        /// Performs an asynchronous GET request for a specific document.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(bool softDeleted)
        {
            ValidateResourceName();
            return await GetAsync<T>(ResourceName, softDeleted);
        }
        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <param name="ifModifiedSince">Return only documents that changed since this date. </param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName, DateTime? ifModifiedSince)
        {
            ValidateResourceName(resourceName);

            HttpResponse = await GetAsync(resourceName, ifModifiedSince);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(List<T>);
            return await ParseJsonAsListOf<T>(HttpResponse.Content);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <param name="softDeleted">Include deleted documents. </param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName, bool softDeleted)
        {
            ValidateResourceName(resourceName);

            HttpResponse = await GetAsync(resourceName, null, null, softDeleted, null);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(List<T>);
            return await ParseJsonAsListOf<T>(HttpResponse.Content);
        }
        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <param name="ifModifiedSince">Return only documents that changed since this date. </param>
        /// <param name="rawQuery">Only return documents matching this query.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName, DateTime? ifModifiedSince, string rawQuery)
        {
            ValidateResourceName(resourceName);

            HttpResponse = await GetAsync(resourceName, null, ifModifiedSince, false, rawQuery);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(List<T>);
            return await ParseJsonAsListOf<T>(HttpResponse.Content);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <param name="softDeleted">Include deleted documents. </param>
        /// <param name="rawQuery">Only return documents matching this query.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName, bool softDeleted, string rawQuery)
        {
            ValidateResourceName(resourceName);

            HttpResponse = await GetAsync(resourceName, null, null, softDeleted, rawQuery).ConfigureAwait(false);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(List<T>);
            return await ParseJsonAsListOf<T>(HttpResponse.Content);
        }


        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <param name="ifModifiedSince">Return only documents that changed since this date. </param>
        /// <param name="softDeleted">Include deleted documents.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName, DateTime? ifModifiedSince, bool softDeleted)
        {
            ValidateResourceName(resourceName);

            HttpResponse = await GetAsync(resourceName, null, ifModifiedSince, softDeleted, null);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(List<T>);
            return await ParseJsonAsListOf<T>(HttpResponse.Content);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <param name="ifModifiedSince">Return only documents that changed since this date. </param>
        /// <param name="softDeleted">Wether soft deleted documents should be returned or not.</param>
        /// <param name="rawQuery">Only return documents matching this query.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName, DateTime? ifModifiedSince, bool softDeleted, string rawQuery)
        {
            ValidateResourceName(resourceName);

            HttpResponse = await GetAsync(resourceName, null, ifModifiedSince, softDeleted, rawQuery);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return default(List<T>);
            return await ParseJsonAsListOf<T>(HttpResponse.Content);
        }

        /// <summary>
        /// Performs an asynchronous GET request on a resource endpoint.
        /// </summary>
        /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
        public async Task<List<T>> GetAsync<T>(string resourceName)
        {
            return await GetAsync<T>(resourceName, ifModifiedSince: null);
        }

        #endregion Overloads
    }
}
