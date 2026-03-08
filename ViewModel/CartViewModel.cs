namespace Bake.ViewModel
{
    public class CartViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImgUrl { get; set; }
        public decimal SubTotal 
        {
            get 
            {
                return Price * Quantity;
            }
        }
    }
}
