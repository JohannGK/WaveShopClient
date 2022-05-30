using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WaveShopClient.Pages.Model;

public partial class Product_Image
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public DateTime LastUpdate { get; set; }
    public int? IdProduct { get; set; }
    public virtual Product? IdProductNavigation { get; set; }
}
