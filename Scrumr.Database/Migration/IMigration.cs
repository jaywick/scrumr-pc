﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scrumr.Database.Migration
{
    public interface IMigration
    {
        bool Upgrade(JObject currentPath);
    }
}
