using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditSprint : EditView
    {
        public EditSprint(ScrumrContext context, Entity entity = null)
            : base(typeof(Sprint), context, entity) { }

        protected override void OnSave(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
