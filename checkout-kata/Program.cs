using checkout_kata.Interface;
using checkout_kata.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace checkout_kata;
internal class Program
{
    static void Main(string[] args)
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        var logFilePath = Path.Combine(projectDirectory, "logs", "logs.txt");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog();
                })
                .AddScoped<CheckoutApplication>()
                .AddScoped<ICheckout, CheckoutTest>()
                .BuildServiceProvider();

        var app = serviceProvider.GetRequiredService<CheckoutApplication>();

        app.Process("ABC");
    }
}

public class CheckoutApplication(ILogger<CheckoutApplication> _logger, ICheckout checkout)
{
    public void Process(string skus)
    {
        if (string.IsNullOrEmpty(skus))
        {
            _logger.LogError("Error - No skus available.");
            throw new ArgumentNullException(skus, "Skus can't be null or empty.");
        }

        foreach (var sku in skus)
        {
            checkout.Scan(sku.ToString());
        }

        var totalPrice = checkout.GetTotalPrice();

        Console.WriteLine($"Total price: {totalPrice}");
    }
}

