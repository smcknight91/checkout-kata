namespace checkout_kata.Models
{
    public class PriceConfigurationModel
    {
        public int Price { get; set; }
        public string SKU { get; set; }
        public List<MultibuyModel>? Multibuy { get; set; }
    }
}
