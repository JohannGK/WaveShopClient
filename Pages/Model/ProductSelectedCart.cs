using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WaveShopClient.Pages.Model;
public partial class ProductSelectedCart
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Status { get; set; }
    public int IdProduct { get; set; }
    public int IdShoppingCart { get; set; }

    [JsonIgnore]
    public virtual Product IdProductNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ShoppingCart IdShoppingCartNavigation { get; set; } = null!;
}
