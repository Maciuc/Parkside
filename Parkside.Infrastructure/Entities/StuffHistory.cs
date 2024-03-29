﻿using System;
using System.Collections.Generic;

namespace Parkside.Infrastructure.Entities
{
    public partial class StuffHistory
    {
        public int Id { get; set; }
        public int StuffId { get; set; }
        public string? Year { get; set; }
        public string? TeamName { get; set; }
        public string? Role { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Stuff Stuff { get; set; } = null!;
    }
}
