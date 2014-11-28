using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    abstract class EditEntityBase<TEntity> : EditView where TEntity : Database.Entity
    {
        public EditEntityBase(ScrumrContext context, TEntity entity = null)
            : base(typeof(TEntity), context, entity) { }

        protected abstract IEnumerable<Expression<Func<TEntity, object>>> OnRender();

        protected abstract void OnUpdated(TEntity entity);

        protected abstract void OnCreated(TEntity entity);

        #region Interface to EditView

        protected sealed override IEnumerable<string> GetRenderableViews()
        {
            foreach (var item in OnRender())
                yield return item.GetExpressedProperty().Name;
        }

        protected sealed override void PostUpdated(Database.Entity entity)
        {
            OnUpdated((TEntity)entity);
        }

        protected sealed override void PostCreated(Database.Entity entity)
        {
            OnCreated((TEntity)entity);
        }

        #endregion
    }
}
