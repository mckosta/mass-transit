using Amazon.SQS;
using MassTransit;
using MassTransit.AmazonSqsTransport.Configurators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace mass_transit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<EventConsumer>(); // Single Message

                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = "localhost:4566";
                    var hostAddress = new Uri($"amazonsqs://{region}");
                    var hostConfigurator = new AmazonSqsHostConfigurator(hostAddress);
                    hostConfigurator.AccessKey("teste");
                    hostConfigurator.SecretKey("teste");
                    //hostConfigurator.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = $"http://{region}" });
                    hostConfigurator.Config(new AmazonSQSConfig { ServiceURL = $"http://{region}" });
                    cfg.Host(hostConfigurator.Settings);

                    cfg.ReceiveEndpoint("teste", e =>
                    {
                        e.ConfigureConsumeTopology = false;
                        e.ClearMessageDeserializers();
                        e.UseRawJsonSerializer();
                        e.PublishFaults = false;
                        e.RethrowFaultedMessages();
                        e.ThrowOnSkippedMessages();
                        e.ConfigureConsumer<EventConsumer>(context); // Single Message

                        // Batch of messages (Message stay stuck, does not reach the consumer)
                        //e.PrefetchCount = 10;
                        //e.Batch<OrderBatch>(b =>
                        //{
                        //    b.MessageLimit = 10;
                        //    b.ConcurrencyLimit = 1;
                        //    b.TimeLimit = TimeSpan.FromSeconds(1);
                        //    b.Consumer(() => new EventConsumerBatch());
                        //});
                    });
                });
            });

            services.AddMassTransitHostedService();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
