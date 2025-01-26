using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordImageViewer.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private readonly string _channelId;
        private TaskCompletionSource _readyTaskCompletionSource;

        // Constructor initializes the client, token, and channel ID
        public DiscordService(string token, string channelId)
        {
            _token = token;
            _channelId = channelId;
            _client = new DiscordSocketClient();
        }

        // Connect the bot to Discord
        public async Task ConnectAsync()
        {
            _client.Log += Log;
            _client.Ready += OnReady;  // Register Ready event

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Initialize TaskCompletionSource and wait for the Ready event
            _readyTaskCompletionSource = new TaskCompletionSource();
        }

        private Task Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage);
            return Task.CompletedTask;
        }

        // This method is triggered once the bot is logged in and fully ready
        private Task OnReady()
        {
            // Mark the TaskCompletionSource as completed, signaling that the bot is ready
            _readyTaskCompletionSource.SetResult();
            Console.WriteLine("Bot is connected and ready!");
            return Task.CompletedTask;
        }

        // Get image URLs from the Discord channel asynchronously
        public async Task<List<string>> GetImageUrlsAsync()
        {
            // Wait until the bot is ready
            await _readyTaskCompletionSource.Task;

            // Convert the channelId string to ulong
            if (!ulong.TryParse(_channelId, out var channelId))
            {
                Console.WriteLine("Invalid channel ID");
                return new List<string>();  // Return empty list if channelId is invalid
            }

            var channel = _client.GetChannel(channelId) as SocketTextChannel;

            if (channel == null)
            {
                Console.WriteLine($"Channel with ID {channelId} not found.");
                return new List<string>();  // Return empty list if channel is not found
            }

            // Fetch the last 100 messages from the channel asynchronously
            var messages = await channel.GetMessagesAsync(limit: 100).FlattenAsync();

            // Filter messages with image attachments and return the URLs
            var imageUrls = messages
                .Where(m => m.Attachments.Any(a => IsImage(a.Filename)))  // Filter only images
                .SelectMany(m => m.Attachments.Select(a => a.Url))  // Select image URLs
                .ToList();

            return imageUrls;
        }

        // Helper function to check if a file is an image based on its extension
        private static bool IsImage(string filename)
        {
            var extensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" };
            return extensions.Any(e => filename.EndsWith(e, StringComparison.OrdinalIgnoreCase));
        }
    }
}
