using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Db;

namespace RockPaperScissors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            ApplyMigrations(host);

            host.Run();

        }

        private static void ApplyMigrations(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<GameDbContext>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ошибка при миграции базы данных");
            }
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