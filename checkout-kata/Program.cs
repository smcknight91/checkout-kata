using checkout_kata.Service;

namespace checkout_kata;

public class CheckoutApplication
{
    static void Main(string[] args)
    {
        Process("!!!!");
    }

    public static void Process(string skus)
    {
        var checkout = new Checkout();

        if (string.IsNullOrEmpty(skus))
        {
            Console.WriteLine("Error - No skus available.");
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