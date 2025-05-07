using System.Net.Security;

public class DevHttpsConnectionHelper
{
    public DevHttpsConnectionHelper(int sslPort, HttpMessageHandler? customHandler = null)
    {
        SslPort = sslPort;
        DevServerRootUrl = FormattableString.Invariant($"https://{DevServerName}:{SslPort}");
        LazyHttpClient = new Lazy<HttpClient>(() =>
        {
            var handler = GetPlatformMessageHandler(customHandler);
            return handler != null ? new HttpClient(handler) : new HttpClient();
        });
    }

    public int SslPort { get; }

    public string DevServerName =>
#if WINDOWS
        "localhost";
#elif ANDROID
        "10.0.2.2";
#else
        throw new PlatformNotSupportedException("Only Windows and Android currently supported.");
#endif

    public string DevServerRootUrl { get; }

    private Lazy<HttpClient> LazyHttpClient;
    public HttpClient HttpClient => LazyHttpClient.Value;

    public HttpMessageHandler? GetPlatformMessageHandler(HttpMessageHandler? customHandler)
    {
        try
        {
#if WINDOWS
            return customHandler;
#elif ANDROID
            var handler = new CustomAndroidMessageHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == SslPolicyErrors.None;
            };

            return handler;
#else
            throw new PlatformNotSupportedException("Only Windows and Android currently supported.");
#endif
        }
        catch (Exception ex)
        {
            throw;
        }
    }

#if ANDROID
    internal sealed class CustomAndroidMessageHandler : Xamarin.Android.Net.AndroidMessageHandler
    {
        protected override Javax.Net.Ssl.IHostnameVerifier GetSSLHostnameVerifier(Javax.Net.Ssl.HttpsURLConnection connection)
            => new CustomHostnameVerifier();

        private sealed class CustomHostnameVerifier : Java.Lang.Object, Javax.Net.Ssl.IHostnameVerifier
        {
            public bool Verify(string? hostname, Javax.Net.Ssl.ISSLSession? session)
            {
                return
                    Javax.Net.Ssl.HttpsURLConnection.DefaultHostnameVerifier.Verify(hostname, session)
                    || hostname == "10.0.2.2" && session.PeerPrincipal?.Name == "CN=localhost";
            }
        }
    }
#endif
}
