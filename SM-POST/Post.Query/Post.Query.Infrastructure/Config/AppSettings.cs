﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Config;

public class AppSettings
{
    public class ConnectionStrings
    {
        public string SqlServer { get; set; } = null!;
    }
}
