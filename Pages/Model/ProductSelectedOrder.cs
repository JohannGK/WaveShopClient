using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WaveShopClient.Pages.Model;
public partial class ProductSelectedOrder
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Status { get; set; }
    public int IdProduct { get; set; }
    public int IdOrder { get; set; }
    public virtual Order IdOrderNavigation { get; set; } = null!;
    public virtual Product IdProductNavigation { get; set; } = null!;
}
