using System;
using System.Collections.Generic;

namespace WaveShopClient.Pages.Model;
public partial class ShoppingCart
{
    public ShoppingCart()
    {
        ProductSelectedCarts = new HashSet<ProductSelectedCart>();
    }

    public int id { get; set; }
    public int productsQuantity { get; set; }
    public double subtotal { get; set; }
    public DateTime LastUpdate { get; set; }
    public int? IdUser { get; set; }

    public virtual ICollection<ProductSelectedCart> ProductSelectedCarts { get; set; }
}
