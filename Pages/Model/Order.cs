using System;
using System.Collections.Generic;

namespace WaveShopClient.Pages.Model;
public partial class Order
{
    public Order()
    {
        ProductSelectedOrders = new HashSet<ProductSelectedOrder>();
    }

    public int Id { get; set; }
    public int IdUser { get; set; }
    public int? IdShoppingCart { get; set; }
    public DateTime Ordered { get; set; }
    public DateTime? Shipped { get; set; }
    public string Status { get; set; } = null!;
    public double Total { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
    public virtual ICollection<ProductSelectedOrder> ProductSelectedOrders { get; set; }
}