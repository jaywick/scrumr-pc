using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditProject : EditView
    {
        public EditProject(ScrumrContext context, Entity entity = null)
            : base(typeof(Project), context, entity) { }
    }
}
