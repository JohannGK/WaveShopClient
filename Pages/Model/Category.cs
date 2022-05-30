﻿using System;
using System.Collections.Generic;

namespace WaveShopClient.Pages.Model;
public partial class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
}
