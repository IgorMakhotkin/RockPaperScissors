using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace RockPaperScissors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.AllowSynchronousIO = true;
                            
                            options.Limits.MaxRequestBodySize = null;
                            
                            options.ListenLocalhost(5000, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1;
                            });
                            
                            options.ListenLocalhost(5001, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                listenOptions.UseHttps(httpsOptions =>
                                {
                                    httpsOptions.SslProtocols =
                                        System.Security.Authentication.SslProtocols.Tls |
                                        System.Security.Authentication.SslProtocols.Tls11 |
                                        System.Security.Authentication.SslProtocols.Tls12;
                                });
                            });
                        });
                });
    }
}