Eve.NET [![Build status](https://ci.appveyor.com/api/projects/status/gvd1i4565flupru6?svg=true)](https://ci.appveyor.com/project/nicolaiarocci/eve-net)
=======
Eve.NET is a simple HTTP and REST client for Web Services powered by the [Eve 
Framework][1]. It leverages both `System.Net.HttpClient` and `Json.NET` to 
provide the best possible Eve experience on the .NET platform.

Cross platform
--------------
Written and maintained by the same author of the Eve Framework, Eve.NET is 
delivered as a portable library (PCL) which runs seamlessly on Xamarin.iOS, 
Xamarin.Android, Windows Phone 8.1 and Windows 8, .NET45. We use Eve.NET 
internally to power our iOS, Web and Windows applications.

Usage
-----

### Initialization
```C#
// Simplest initialization possible.
var client = new EveClient();
client.BaseAddess = new Uri("http://api.com");

// or
var client = new EveClient { BaseAddress = new Uri("http://api.com") };

// or!
var client = new EveClient { 
    BaseAddress = new Uri("http://api.com"), 
    BasicAuthenticator = new BasicAuthenticator  ("user", "pw")
};

// Set target resouce for subsequent requests.
client.ResourceName = "companies";
````
### GET at Resource Endpoints
```C#
// Returns a List<T>.
var companies = await client.GetAsync<Company>();

Assert.AreEqual(HttpStatusCode.OK, client.HttpResponse.StatusCode);
Assert.AreEqual(companies.Count, 10);

// Returns a List<T> which only includes changed items since a DateTime.
var ifModifiedSince = DateTime.Now.AddDays(-1);
var companies = await client.GetAsync<Company>(ifModifiedSince);

Assert.AreEqual(HttpStatusCode.OK, client.HttpResponse.StatusCode);
Assert.AreEqual(companies.Count, 2);
```
### GET at Document Endpoints
```C#
var company = companies[0];

// Update an existing object silently performing a If-None-Match request based
// on object ETag.  See http://python-eve.org/features#conditional-requests
company = await client.GetAsync<Company>(company);

// StatusCode is 'NotModified' since ETag matches the one on the server (no
// download was performed). Would be OK if a download happened. Object did not
// change.
Assert.AreEqual(HttpStatusCode.NotModified, client.HttpResponse.StatusCode);


// Raw, conditional GET request
var companyId = "507c7f79bcf86cd7994f6c0e";
var eTag = "7776cdb01f44354af8bfa4db0c56eebcb1378975";

var company = await client.GetAsync<Company>("companies", companyId, eTag);

// HttpStatusCode is still 'NotModified'.
Assert.AreEqual(HttpStatusCode.NotModified, client.HttpResponse.StatusCode);
```
### POST/Create Requests
```C#
var company = await client.PostAsync<Company>(new Company { Name = "MyCompany" });

// HttpStatusCode is 'Created'.
Assert.AreEqual(HttpStatusCode.Created, client.HttpResponse.StatusCode);
Assert.AreEqual("MyCompany", company.Name);

// Newly created object includes properly initialized API metafields.
Assert.IsInstanceOf<DateTime>(company.Created);
Assert.IsInstanceOf<DateTime>(company.Updated);
Assert.IsNotNullOrEmpty(company.UniqueId);
Assert.IsNotNullOrEmpty(company.ETag);
```
### PUT/Replace Requests
```C#
company.Name = "YourCompany";

// PUT requests will silently perform a If-Match request so server copy will only be
// updated if server and document ETag match.
// See http://python-eve.org/features#data-integrity-and-concurrency-control
var result = await client.PutAsync<Company>(company);

Assert.AreEqual(HttpStatusCode.OK, client.HttpResponse.StatusCode);
Assert.AreEqual(result.Name, company.Name);

// UniqueId and Created did not change.
Assert.AreEqual(result.UniqueId, company.UniqueId);
Assert.AreEqual(result.Created, company.Created);

// However Updated and ETag have been updated.
Assert.AreNotEqual(result.Updated, company.Updated);
Assert.AreNotEqual(result.ETag, company.ETag);
```
### DELETE Requests
```C#
// DELETE requests will silently perform a If-Match request so document
// will only be deleted if its ETag matches the one on the server.
// See http://python-eve.org/features#data-integrity-and-concurrency-control
var message = await client.DeleteAsync(Original);
Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
```
### Mapping JSON and Eve meta-fields to Classes
```C#
// You want to map Eve meta-fields to class properties by flagging them with
// the RemoteAttribute. Since these are usually consistent across API
// endpoints it is probably a good idea to provide a base class for your
// business objects to inherit from.
public abstract class BaseClass
{
    [JsonProperty("_id")]
    [Remote(Meta.DocumentId)]
    public string UniqueId { get; set; }

    [JsonProperty("_etag")]
    [Remote(Meta.ETag)]
    public string ETag { get; set; }

    [JsonProperty("_updated")]
    [Remote(Meta.LastUpdated)]
    public DateTime Updated { get; set; }

    [JsonProperty("_created")]
    [Remote(Meta.DateCreated)]
    public DateTime Created { get; set; }
}

// In both JSON and MongoDB it is common and good practice to adopt short field
// names so we map those to our streamlined class properties.
public class Company : BaseClass
{
    [JsonProperty("n")]
    public string Name { get; set; }

    [JsonProperty("p")]
    public string Password { get; set; }
}
```
### Raw GET Requests
```C#
// You can use this method to perform parametrized queries.
var query = @"companies?where={""n"": ""MyCompany""}";
// GetAsync will return a HttpResponseMessage which you can freely inspect.
var response = await client.GetAsyc(query);

Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
Assert.AreEqual("application/json", result.Content.Headers.ContentType.ToString())

// Please note that you also get the HttpResponseMessage object with GetAsync<T> requests, 
// exposed by the HttpResponse property.
```
Installation
------------
Eve.NET is on [NuGet][9]. Run the following command on the Package Manager Console:

```
PM> Install-Package Eve.NET
```
Or install via the NuGet Package Manager in Visual/Xamarin Studio. 

Running the tests
-----------------
You are supposed to  clone the [`evenet-testbed`][7] repo and run a local
instance of the test webservice. Alternatively, if you don't have a Python/Eve
environmnet at hand, you can opt to rely on a free (and slow, and very resource
limited) test instance which is available online. See [tests code][8] for
details.

License
-------
Eve.NET is a [Nicola Iarocci][2] and [Gestionali Amica][3] open source project,
distributed under the [BSD license][4].

[1]: http://python-eve.org
[2]: http://nicolaiarocci.com
[3]: http://gestionaleamica.com
[4]: https://github.com/nicolaiarocci/Eve.NET/blob/master/LICENSE.txt
[5]: http://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx
[6]: http://james.newtonking.com/json
[7]: https://github.com/nicolaiarocci/Eve.NET-testbed
[8]: https://github.com/nicolaiarocci/Eve.NET/blob/master/Eve.Client.Tests/MethodsBase.cs#L13-L31
[9]: https://www.nuget.org/packages/Eve.NET/
