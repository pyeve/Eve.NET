using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve
{
    public partial class EveClient : IDisposable
    {
        /// <summary>
        /// Performs an asynchronous DELETE request on a document endpoint.
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="obj">Object to be deleted on the service.</param>
        public async Task<HttpResponseMessage> DeleteAsync(string resourceName, object obj)
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
                SettingsForEditing(client, obj);
                HttpResponse = await client.DeleteAsync(string.Format("{0}/{1}", resourceName, GetDocumentId(obj))).ConfigureAwait(false);
                return HttpResponse;
            }
        }

        /// <summary>
        /// Performs an asynchronous DELETE request on a document endpoint
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <param name="obj">Object to be deleted from the service.</param>
        public async Task<HttpResponseMessage> DeleteAsync(object obj)
        {
            ValidateResourceName();
            HttpResponse = await DeleteAsync(ResourceName, obj);
            return HttpResponse;
        }

        /// <summary>
        /// Performs an asynchronous DELETE request on a resource endpoint
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <remarks>Use with caution as this is likely to delete all endpoint data.</remarks>
        public async Task<HttpResponseMessage> DeleteAsync()
        {
            ValidateBaseAddress();
            ValidateResourceName();

            using (var client = new HttpClient())
            {
                Settings(client);
                HttpResponse = await client.DeleteAsync(ResourceName).ConfigureAwait(false);
                return HttpResponse;
            }
        }

        /// <summary>
        /// Performs an asynchronous DELETE request on a resource endpoint
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <param name="resourceName">Resource endpoint.</param>
        /// <remarks>Use with caution as this is likely to delete all endpoint data.</remarks>
        public async Task<HttpResponseMessage> DeleteAsync(string resourceName)
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

            using (var client = new HttpClient())
            {
                Settings(client);
                HttpResponse = await client.DeleteAsync(resourceName).ConfigureAwait(false);
                return HttpResponse;
            }
        }
    }
}
