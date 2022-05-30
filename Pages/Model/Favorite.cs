using System;
using System.Collections.Generic;

namespace WaveShopClient.Pages.Model;
public partial class Favorite
{
    public int IdUser { get; set; }
    public int IdProduct { get; set; }
    public DateTime Creation { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
    public virtual User IdUserNavigation { get; set; } = null!;
}
