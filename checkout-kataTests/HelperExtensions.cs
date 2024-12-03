using checkout_kata.Service;

namespace checkout_kataTests
{
    public static class HelperExtensions
    {
        public static void ScanSkus(Checkout checkout, string skus)
        {
            foreach (var sku in skus)
            {
                checkout.Scan(sku.ToString());
            }
        }
    }
}
