namespace checkout_kata.Interface
{
    public interface ICheckout
    {
        void Scan(string item);
        int GetTotalPrice();
    }
}
