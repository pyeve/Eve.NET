using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve
{
    public partial class EveClient : IDisposable
    {
        /// <summary>
        /// Performs an asynchronous POST request on a resource endpoint.
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="obj">Object to be stored.</param>
        public async Task<HttpResponseMessage> PostAsync(string resourceName, object obj)
        {
            ValidateBaseAddress();
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            if (resourceName == string.Empty)
            {
                throw new ArgumentException("resourceName");
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (var client = new HttpClient())
            {
                Settings(client);
                HttpResponse = await client.PostAsync(resourceName, SerializeObject(obj)).ConfigureAwait(false);
                return HttpResponse;
            }
        }

        /// <summary>
        /// Performs an asynchronous POST request on a resource endpoint.
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <param name="obj">Object to be stored.</param>
        public async Task<HttpResponseMessage> PostAsync(object obj)
        {
            ValidateResourceName();
            return await PostAsync(ResourceName, obj);
        }

        /// <summary>
        /// Performs an asynchronous POST request on a resource endpoint.
        /// </summary>
        /// <returns>An instance of the document.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="obj">Object to be stored on the service.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        public async Task<T> PostAsync<T>(string resourceName, object obj)
        {
            HttpResponse = await PostAsync(resourceName, obj).ConfigureAwait(continueOnCapturedContext: false);

            switch (HttpResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                    return await DeserializeObject<T>(HttpResponse);
                default:
                    return default(T);
            }
        }

        /// <summary>
        /// Performs an asynchronous POST request on a resource endpoint.
        /// </summary>
        /// <returns>An instance of the document.</returns>
        /// <param name="obj">Object to be stored on the service.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        public async Task<T> PostAsync<T>(object obj)
        {
            ValidateResourceName();
            return await PostAsync<T>(ResourceName, obj);
        }

        /// <summary>
        /// Performs an asynchronous POST request on a resource endpoint. If
        /// one of the documents is rejected by the service, the whole batch
        /// is rejected and no document is stored.
        /// </summary>
        /// <returns>The objects stored.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="objs">Objects to be stored on the service.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        public async Task<List<T>> PostAsync<T>(string resourceName, IEnumerable<T> objs)
        {
            ValidateBaseAddress();
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            if (resourceName == string.Empty)
            {
                throw new ArgumentException("resourceName");
            }
            if (objs == null)
            {
                throw new ArgumentNullException("objs");
            }

            using (var client = new HttpClient())
            {
                Settings(client);
                HttpResponse = await client.PostAsync(resourceName, SerializeObject(objs)).ConfigureAwait(false);

                if (HttpResponse.StatusCode != HttpStatusCode.Created)
                    return default(List<T>);
                return await ParseJsonAsListOf<T>(HttpResponse.Content);
            }
        }

        /// <summary>
        /// Performs an asynchronous POST request on a resource endpoint. If
        /// one or more document fail validation and are rejected, the whole
        /// batch is rejected and no document is stored on the service.
        /// </summary>
        /// <returns>An instance of the document.</returns>
        /// <param name="objs">Object to be stored on the service.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        public async Task<List<T>> PostAsync<T>(IEnumerable<T> objs)
        {
            return await PostAsync(ResourceName, objs).ConfigureAwait(false);
        }
    }
}
