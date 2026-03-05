namespace Bake.ViewModel
{
    public class CartViewModel
    {
        public int ProdductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string ImgUrl { get; set; }
        public int SubTotal 
        {
            get 
            {
                return Price * Quantity;
            }
        }
    }
}
