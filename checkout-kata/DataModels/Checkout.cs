namespace checkout_kata.DataModels
{
    public class Checkout : ICheckout
    {
        public Dictionary<string, int> scannedItemCount = new Dictionary<string, int>();
        public List<PriceConfigurationModel> priceConfigurations =
            [
                new PriceConfigurationModel { SKU = "A", Price = 50 },
                new PriceConfigurationModel { SKU = "B", Price = 30 },
                new PriceConfigurationModel { SKU = "C", Price = 20 },
                new PriceConfigurationModel { SKU = "D", Price = 15 }
            ];

        public void Scan(string item)
        {
            AddItemAndCountToDictionary(item);

            Console.WriteLine($"Added {item} to scannedItemCount.");
        }

        public int GetTotalPrice()
        {
            var totalPrice = 0;
            foreach (var sku in scannedItemCount.Keys)
            {
                var priceConfig = priceConfigurations.Find(pc => pc.SKU == sku);
                if (priceConfig != null)
                {
                    totalPrice += priceConfig.Price * scannedItemCount[sku];
                }
            }
            return totalPrice;
        }

        private void AddItemAndCountToDictionary(string item)
        {
            if (priceConfigurations.All(priceConfig => priceConfig.SKU != item))
            {
                Console.WriteLine($"Error - Scanned {item} does not exist.");
                throw new KeyNotFoundException("Scanned SKU does not exist.");
            }

            if (scannedItemCount.ContainsKey(item))
            {
                scannedItemCount[item]++;
            }
            else
            {
                scannedItemCount[item] = 1;
            }
        }
    }
}
