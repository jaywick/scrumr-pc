﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditFeature : EditView
    {
        public EditFeature(Context context, Entity entity = null)
            : base(typeof(Feature), context, entity) { }
    }
}
