using Receiver.Services;

namespace Receiver
{
    public class Startup
    {


            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddGrpc();

                //services.AddSingleton<IConnectionStorageService, ConnectionStorageService>();
                // services.AddHostedService<SenderWorker>();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseRouting();


                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<NotificationServer>();

                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
                });
            }
        }
    
}
