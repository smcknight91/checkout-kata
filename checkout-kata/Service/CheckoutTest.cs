using checkout_kata.Interface;
using checkout_kata.Models;
using Microsoft.Extensions.Logging;

namespace checkout_kata.Service
{
    public class CheckoutTest : ICheckout
    {
        private readonly ILogger _logger;
        public CheckoutTest(ILogger<CheckoutTest> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, int> ScannedItemDetails { get; } = new();

        private readonly List<PriceConfigurationModel> _priceConfigurations =
        [
            new PriceConfigurationModel
                { SKU = "A", Price = 50, Multibuy = [new MultibuyModel { ItemCount = 3, TotalPrice = 130 }, new MultibuyModel { ItemCount = 6, TotalPrice = 200 }]},
            new PriceConfigurationModel
                { SKU = "B", Price = 30, Multibuy = [new MultibuyModel { ItemCount = 2, TotalPrice = 45 }, new MultibuyModel { ItemCount = 4, TotalPrice = 30 }]},
            new PriceConfigurationModel
                { SKU = "C", Price = 20, Multibuy = [new MultibuyModel { ItemCount = 6, TotalPrice = 100 }]},
            new PriceConfigurationModel { SKU = "D", Price = 15, Multibuy = null }
        ];

        public void Scan(string item)
        {
            if (_priceConfigurations.All(priceConfig => priceConfig.SKU != item))
            {
                _logger.LogError($"Error - Scanned {item} does not exist.");
                throw new KeyNotFoundException("Scanned SKU does not exist.");
            }

            if (ScannedItemDetails.ContainsKey(item))
            {
                ScannedItemDetails[item]++;
            }
            else
            {
                ScannedItemDetails[item] = 1;
            }
            _logger.LogInformation($"Added {item} to scannedItemCount.");
        }

        public int GetTotalPrice()
        {
            var totalPrice = 0;
            foreach (var sku in ScannedItemDetails.Keys)
            {
                var priceConfig = _priceConfigurations.Find(pc => pc.SKU == sku);
                var remainingItemCount = ScannedItemDetails[sku];

                if (priceConfig.Multibuy != null)
                {
                    var selectedMultibuy = GetMultibuyModel(priceConfig, remainingItemCount);
                    while (selectedMultibuy != null)
                    {
                        totalPrice += selectedMultibuy.TotalPrice;
                        remainingItemCount -= selectedMultibuy.ItemCount;
                        selectedMultibuy = GetMultibuyModel(priceConfig, remainingItemCount);
                    }
                }
                totalPrice += remainingItemCount * priceConfig.Price;
            }
            return totalPrice;
        }

        private MultibuyModel? GetMultibuyModel(PriceConfigurationModel priceConfig, int remainingItemCount)
        {
            return priceConfig.Multibuy.Where(pc => pc.ItemCount <= remainingItemCount)
                .OrderByDescending(pc => pc.ItemCount).FirstOrDefault();
        }
    }
}
