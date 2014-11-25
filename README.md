Eve.NET
=======
Eve.NET is a simple cross platform .NET client for Web Services powered by the
[Eve REST API framework][1]. It leverages both [`System.Net.HttpClient`][5] and 
[`Json.NET`][6] to provide the best possible Eve experience on .NET.

Cross platform
--------------
Eve.NET is delivered as portable (PCL) library and runs semalessly on .NET4,
Mono, Xamarin.iOS, Xamarin.Android, Windows Phone and Windows 8. We use Eve.NET
to power our iOS applications which of course consume a remote Eve-powered API.

Running the tests
-----------------
You are supposed to  clone the [`evenet-testbed`][7] repo and run a local instance
of the test webservice. Alternatively, if you don't have a Python/Eve environmnet
at hand, you can opt to rely on a free (and slow, and very resource limited) test
instance which is available online. See [tests code][8] for details.
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
