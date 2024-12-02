namespace checkout_kata.DataModels
{
    public interface ICheckout
    {
        void Scan(string item);
        int GetTotalPrice();
    }
}
