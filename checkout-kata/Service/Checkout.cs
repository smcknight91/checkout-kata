using checkout_kata.Interface;
using checkout_kata.Models;

namespace checkout_kata.Service
{
    public class Checkout : ICheckout
    {
        public Dictionary<string, int> ScannedItemDetails = new();

        private readonly List<PriceConfigurationModel> _priceConfigurations =
        [
            new PriceConfigurationModel
                { SKU = "A", Price = 50, Multibuy = new MultibuyModel { ItemCount = 3, TotalPrice = 130 } },
            new PriceConfigurationModel
                { SKU = "B", Price = 30, Multibuy = new MultibuyModel { ItemCount = 2, TotalPrice = 45 } },
            new PriceConfigurationModel
                { SKU = "C", Price = 20, Multibuy = new MultibuyModel { ItemCount = 6, TotalPrice = 100 } },
            new PriceConfigurationModel { SKU = "D", Price = 15, Multibuy = null }
        ];

        public void Scan(string item)
        {
            if (_priceConfigurations.All(priceConfig => priceConfig.SKU != item))
            {
                Console.WriteLine($"Error - Scanned {item} does not exist.");
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

            Console.WriteLine($"Added {item} to scannedItemCount.");
        }

        public int GetTotalPrice()
        {
            var totalPrice = 0;
            foreach (var sku in ScannedItemDetails.Keys)
            {
                var priceConfig = _priceConfigurations.Find(pc => pc.SKU == sku);

                if (priceConfig == null) continue;

                if (priceConfig.Multibuy != null)
                {
                    var multibuyCount = ScannedItemDetails[sku] / priceConfig.Multibuy.ItemCount;
                    var remainingItemCount = ScannedItemDetails[sku] % priceConfig.Multibuy.ItemCount;

                    totalPrice += multibuyCount * priceConfig.Multibuy.TotalPrice +
                                  remainingItemCount * priceConfig.Price;
                }
                else
                {
                    totalPrice += ScannedItemDetails[sku] * priceConfig.Price;
                }
            }
            return totalPrice;
        }
    }
}
