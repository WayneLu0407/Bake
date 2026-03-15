namespace Bake.Models.Sales;

public class ProductIngredient
{
    public int ProductId { get; set; }           // FK → Products

    public string? ShelfLifeNote { get; set; }   // 賞味期限說明（例：收到後20天）

    public string? Ingredients { get; set; }      // 內容物

    public string? NetWeight { get; set; }        // 內容量（例：30.0g x 12）

    // 導航屬性
    public Product Product { get; set; } = null!;
}