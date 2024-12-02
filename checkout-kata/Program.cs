using checkout_kata.DataModels;
namespace checkout_kata;

public class CheckoutApplication
{
    static void Main(string[] args)
    {
        Process("ABC");
    }

    public static void Process(string skus)
    {
        var checkout = new Checkout();

        foreach (var sku in skus)
        {
            checkout.Scan(sku.ToString());
        }

        var totalPrice = checkout.GetTotalPrice();

        Console.WriteLine($"Total price: {totalPrice}");
    }
}