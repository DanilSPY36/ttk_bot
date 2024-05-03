using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using ttk_bot;
using ttk_bot.Models;



class Program
{
    public IConfiguration Configuration { get; }
    private static ITelegramBotClient _botClient = null!;
    static async Task Main()
    {
        var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();



    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(Configuration);

        services.AddDbContext<TgBotContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
    }
}