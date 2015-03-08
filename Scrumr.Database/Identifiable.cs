﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Database
{
    public abstract class Identifiable
    {
        [JsonIgnore]
        public ScrumrContext Context { get; internal set; }

        [Primary]
        public int ID { get; set; }

        public Identifiable(ScrumrContext context)
        {
            Context = context;
        }
    }
}