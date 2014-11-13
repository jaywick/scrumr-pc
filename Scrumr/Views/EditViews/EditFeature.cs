using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    class EditFeature : EditView
    {
        public EditFeature(ScrumrContext context, Entity entity = null)
            : base(typeof(Feature), context, entity) { }

        protected override void OnSave(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
