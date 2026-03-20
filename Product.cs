namespace InventoryAppLocalDb_EF;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = "";

    public bool IsLowStock => Stock < 10;

    public Product(int id, string name, double price, int stock, string category)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
        Category = category;
    }
    public Product() { }
}