﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public class Meta : Identifiable
    {
        [Primary]
        public int ID { get; set; }

        public int SchemaVersion { get; set; }
    }
}
