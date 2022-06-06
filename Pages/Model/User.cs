using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WaveShopClient.Pages.Model;
public partial class User
{
    public User()
    {
        Addresses = new HashSet<Address>();
        Favorites = new HashSet<Favorite>();
        Orders = new HashSet<Order>();
    }

    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime BirthDay { get; set; }
    public int Age { get; set; }
    public string UserType { get; set; } = null!;
    public string Reputation { get; set; } = null!;
    public DateTime LastLogin { get; set; }
    public DateTime LastUpdate { get; set; }

    public virtual ICollection<Address> Addresses { get; set; }
    public virtual ICollection<Favorite> Favorites { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
}
