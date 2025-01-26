using DiscordImageViewer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Adds MVC services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Your Razor components (if any)

// Register DiscordService as a singleton with the bot token and channel ID from configuration
var discordToken = builder.Configuration["Discord:Token"];
var channelId = builder.Configuration["Discord:ChannelId"];
builder.Services.AddSingleton(new DiscordService(discordToken, channelId));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Set up MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Connect to Discord and retrieve image URLs
var discordService = app.Services.GetRequiredService<DiscordService>();
await discordService.ConnectAsync();
var imageUrls = await discordService.GetImageUrlsAsync();

// Example: You can save the image URLs or pass them to a controller or view
// This could be saved into a service, a database, or used directly in a view
app.MapGet("/images", () => imageUrls);

// Run the app
app.Run();
