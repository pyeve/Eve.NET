Eve.NET
=======
Eve.NET is a simple cross platform client for Web Services powered by the [Eve
REST API framework][1]. It leverages both [`System.Net.HttpClient`][5] and
[`Json.NET`][6] to provide the best possible Eve experience on .NET.

Cross platform
--------------
Eve.NET is delivered as a portable (PCL) library and runs seamlessly on .NET4,
Mono, Xamarin.iOS, Xamarin.Android, Windows Phone and Windows 8. We use Eve.NET
internally to power our iOS and Windows applications.

Usage
-----
```C#

// INITIALIZATION

var client = new EveClient();
client.BaseAddess = new Uri("http://api.com");

// BaseAddress on initialization
var client = new EveClient { BaseAddress = new Uri("http://api.com") };

// Authenticator instance can also be passed on inititalization
var client = new EveClient { 
    BaseAddress = new Uri("http://api.com"), 
    BasicAuthenticator = new BasicAuthenticator  ("user", "pw")
};


// GET AT RESOURCE ENDPOINT
client.ResourceName = "companies";

// List<T>
var companies = await client.GetAsync<Company>();
Assert.AreEqual(HttpStatusCode.OK, client.HttpResponse.StatusCode);
Assert.AreEqual(companies.Count, 10);

// List<T> with only the items that changed since a DateTime.
var ifModifiedSince = DateTime.Now.AddDays(-1);

var companies = await client.GetAsync<Company>(ifModifiedSince);
Assert.AreEqual(HttpStatusCode.OK, client.HttpResponse.StatusCode);
Assert.AreEqual(companies.Count, 2);

// GET TO DOCUMENT ENDPOINT
var company = companies[0];

// Update an existing object silently performing a If-None-Match
// request based on object ETag. 
// See http://python-eve.org/features#conditional-requests
company = await client.GetAsync<Company>(target);

// StatusCode is NotModified since ETag matches the one on the 
// server (no download was performed). Would be OK if a download
// happened. Object did not change.
Assert.AreEqual(HttpStatusCode.NotModified, client.HttpResponse.StatusCode);

// Raw, conditional GET request
var companyId = "507c7f79bcf86cd7994f6c0e";
var eTag = "7776cdb01f44354af8bfa4db0c56eebcb1378975";

var company = await client.GetAsync<Company>("companies", companyId, eTag);
Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode);

```

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
