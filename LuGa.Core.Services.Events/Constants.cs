namespace LuGa.Core.Services.Events
{
    public class Constants
    {
        public const string Environment = "ASPNETCORE_ENVIRONMENT";
        
        public const string Password = "mqtt:password";
        public const string Username = "mqtt:username";
        public const string ClientId = "mqtt:clientid";
        public const string Host = "mqtt:host";
        public const string Port = "mqtt:port";
        
        public const string ServiceName = "luga-events";

        public const string ConnectionString = "LuGa";

        public const int ReconnectDelay = 5;
        
        public const string SubscribeTopic = "devices/#";
        public const string MessageTopic1 = "/set";
        public const string MessageTopic2 = "$";
        
        public const string SplitCharacter = "/";
        
        public const string ConnectedOutput = "### CONNECTED WITH SERVER ###";
        public const string DisconnectedOutput = "### DISCONNECTED WITH SERVER ###";
        public const string StartedOutput = "Started the service";
        public const string StoppedOutput = "Stopped the service";
    }
}