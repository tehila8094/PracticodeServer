﻿using System;
using System.Collections.Generic;

namespace ToDoApi;

public partial class Items
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsComplete { get; set; }
}
