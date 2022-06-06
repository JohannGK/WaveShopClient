using System;
using System.Collections.Generic;

namespace WaveShopClient.Pages.Model;
public partial class Product
{
    public Product()
    {
        Comments = new HashSet<Comment>();
        Favorites = new HashSet<Favorite>();
        ProductSelectedCarts = new HashSet<ProductSelectedCart>();
        ProductSelectedOrders = new HashSet<ProductSelectedOrder>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? PhotoAddress { get; set; }
    public string? VideoAddress { get; set; }
    public int StockQuantity { get; set; }
    public double UnitPrice { get; set; }
    public string Status { get; set; } = null!;
    public DateTime Published { get; set; }
    public string Country { get; set; } = null!;
    public string Location { get; set; } = null!;
    public int? IdCategory { get; set; }
    public int IdVendor { get; set; }
    public int LikesNumber { get; set; }
    public int DislikesNumber { get; set; }
    public int ShoppedTimes { get; set; }
    public int CommentsNumber { get; set; }
    public DateTime LastUpdate { get; set; }
    public string? VendorUsername { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<Favorite> Favorites { get; set; }
    public virtual ICollection<ProductSelectedCart> ProductSelectedCarts { get; set; }
    public virtual ICollection<ProductSelectedOrder> ProductSelectedOrders { get; set; }
    public virtual ICollection<Product_Image> Product_Images { get; set; }
}
