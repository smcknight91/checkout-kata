using checkout_kata.Interface;
using checkout_kata.Models;
using Microsoft.Extensions.Logging;

namespace checkout_kata.Service
{
    public class Checkout : ICheckout
    {
        private readonly ILogger _logger;
        public Checkout(ILogger<Checkout> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, int> ScannedItemDetails { get; } = new();

        private readonly List<PriceConfigurationModel> _priceConfigurations = [];

        public void AddConfigurations(List<PriceConfigurationModel> priceConfigurationList)
        {
            foreach (var priceConfig in priceConfigurationList)
            {
                AddConfigurations(priceConfig);
            }
        }

        public void AddConfigurations(PriceConfigurationModel priceConfigurationModel)
        {
            if (_priceConfigurations.Any(pc => pc.SKU == priceConfigurationModel.SKU))
            {
                _logger.LogError("Error - SKU already exists within configuration.");
                throw new Exception("Error - SKU already exists within configuration.");
            }

            _priceConfigurations.Add(priceConfigurationModel);
        }

        public void AddMultibuyConfigurations(string sku, MultibuyModel multiBuyModel)
        {
            var priceConfigurationModel = _priceConfigurations.Find(priceConfig => priceConfig.SKU == sku);
            
            if (priceConfigurationModel == null)
            {
                _logger.LogError($"Error - Scanned {sku} does not exist.");
                throw new KeyNotFoundException("Scanned SKU does not exist.");
            }

            if (priceConfigurationModel.Multibuy.Any(pc => pc.ItemCount == multiBuyModel.ItemCount))
            {
                _logger.LogError($"Multibuy model exists already for item count for sku: {sku}");
                throw new Exception($"Multibuy model exists already for item count for sku: {sku}");
            }

            priceConfigurationModel.Multibuy.Add(multiBuyModel);
        }

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
