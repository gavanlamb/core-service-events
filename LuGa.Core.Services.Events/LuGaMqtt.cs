using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuGa.Core.Device.Models;
using LuGa.Core.Repository;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.ManagedClient;
using Topshelf;

namespace LuGa.Core.Services.Events
{
    public class LuGaMqtt : ServiceControl
    {
        private readonly IManagedMqttClient client;
        
        private readonly IRepository<Event> eventRepository;
        
        public LuGaMqtt(
            LuGaMqttConfig config,
            IRepository<Event> eventRepository)
        {
            this.eventRepository = eventRepository;
            
            var factory = new MqttFactory();

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(Constants.ReconnectDelay))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(config.ClientId)
                    .WithTcpServer(config.Host, config.Port)
                    .WithCredentials(config.Username, config.Password)
                    .Build())
                .Build();

            client = factory.CreateManagedMqttClient();

            client.ApplicationMessageReceived += (s, e) =>
            {
                if (e.ApplicationMessage.Topic.IndexOf(Constants.MessageTopic1, StringComparison.Ordinal) <= -1 ||
                    e.ApplicationMessage.Topic.IndexOf(Constants.MessageTopic2, StringComparison.Ordinal) != -1) return;

                var pulled = e.ApplicationMessage.Topic.Split(Constants.SplitCharacter);

                var @event = new Event() {
                    DeviceId = pulled[1],
                    Action = pulled[3].Split("_").ElementAt(0),
                    Zone = pulled[3].Split("_").ElementAt(1),
                    Value = Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
                    TimeStamp = DateTime.UtcNow
                };

                this.eventRepository.Add(@event);
            };

            client.Connected += async (s, e) =>
            {
                Debug.WriteLine(Constants.ConnectedOutput);
                await client.SubscribeAsync(
                    new TopicFilterBuilder()
                        .WithTopic(Constants.SubscribeTopic)
                        .Build()
                );
            };

            client.Disconnected += (s, e) => {
                Debug.WriteLine(Constants.DisconnectedOutput);
            };

            Task.Run(() => Background(options));
        }

        public bool Start(HostControl hostControl)
        {
            Debug.WriteLine(Constants.StartedOutput);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Debug.WriteLine(Constants.StoppedOutput);

            return true;
        }
        
        async Task Background(ManagedMqttClientOptions options)
        {
            await client.StartAsync(options);
        }
    }
}