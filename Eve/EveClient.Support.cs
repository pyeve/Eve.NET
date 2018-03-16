using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Eve
{
    public partial class EveClient : IDisposable
    {
        /// <summary>
        /// Sets the default client settings needed by GET and POST request methods.
        /// </summary>
        /// <param name="client">HttpClient instance.</param>
        /// <remarks>>Does not handle the If-Match header.</remarks>
        private void Settings(HttpClient client)
        {
            client.BaseAddress = BaseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (Authenticator != null)
            {
                client.DefaultRequestHeaders.Authorization = Authenticator.AuthenticationHeader();
            }
            foreach (var header in CustomHeaders)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

        }

        /// <summary>
        /// Sets the default client settings needed by PUT and DELETE request methods.
        /// </summary>
        /// <param name="client">HttpClient instance.</param>
        /// <param name="obj">Object to be edited.</param>
        /// <remarks>>Adds the object ETag to the request headers so edit operations can perform successfully.</remarks>
        private void SettingsForEditing(HttpClient client, object obj)
        {
            Settings(client);

            var etag = GetETag(obj);
            if (etag == null)
                throw new ArgumentNullException("ETag");

            client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", etag);
        }

        /// <summary>
        /// Serializes an object to JSON and provides it as a StringContent object which can be used by any request method.
        /// </summary>
        /// <returns>A StringContent instance.</returns>
        /// <param name="obj">The object to be serialized.</param>
        private static StringContent SerializeObject(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new EveContractResolver(),
                DateFormatString = CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern
            };
            var s = JsonConvert.SerializeObject(obj, settings);
            var content = new StringContent(s);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
        private async static Task<T> DeserializeObject<T>(HttpResponseMessage response)
        {
            var s = await response.Content.ReadAsStringAsync();
            var settings = new JsonSerializerSettings() { ContractResolver = ContractResolver };
            T instance = JsonConvert.DeserializeObject<T>(s, settings);
            return instance;
        }

        /// <summary>
        /// Returns the document identifier by which the document is known on the service.
        /// </summary>
        /// <returns>The document identifier.</returns>
        /// <param name="obj">The object to be sent to the service.</param>
        private static string GetDocumentId(object obj)
        {
            var id = GetRemoteMetaFieldValue(obj, Meta.DocumentId);

            if (id == null)
                throw new ArgumentNullException("DocumentId");

            return id;
        }

        /// <summary>
        /// Returns the document ETag which is needed by edit operations on the service.
        /// </summary>
        /// <returns>The document Etag.</returns>
        /// <param name="obj">The object to be sent to the sent to the service.</param>
        private static string GetETag(object obj)
        {
            return GetRemoteMetaFieldValue(obj, Meta.ETag);
        }

        /// <summary>
        /// Returns the value of an object meta field.
        /// </summary>
        /// <returns>The remote meta field value.</returns>
        /// <param name="obj">The object.</param>
        /// <param name="metaField">Meta field to be returned.</param>
        private static string GetRemoteMetaFieldValue(object obj, Meta metaField)
        {
            var pInfo = obj.GetType().GetRuntimeProperties().Where(
                            p => p.IsDefined(typeof(RemoteAttribute), true)).ToList();

            foreach (var p in pInfo)
            {
                var attr = (RemoteAttribute)p.GetCustomAttributes(typeof(RemoteAttribute), true).FirstOrDefault();
                if (attr != null && attr.Field == metaField)
                {
                    var v = p.GetValue(obj, null);
                    return (v == null) ? null : v.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Validates the name of the resource.
        /// </summary>
        private void ValidateResourceName(string resource = "<IDontThinkSo>")
        {
            string field, challenge;
            if (resource == "<IDontThinkSo>")
            {
                challenge = ResourceName;
                field = "ResourceName";
            }
            else
            {
                challenge = resource;
                field = "resourceName";
            }
            if (challenge == null)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException(field);
            }
            if (challenge == string.Empty)
            {
                throw new ArgumentException(string.Format("{0} cannot be empty.", field));
            }
        }

        /// <summary>
        /// Validates the base address.
        /// </summary>
        private void ValidateBaseAddress()
        {
            if (BaseAddress == null)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("BaseAddress");
            }
        }
    }
}
