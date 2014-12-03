using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr.Client
{
    abstract class EditEntityBase<TEntity> where TEntity : Entity
    {
        private EditView _view;

        public EditEntityBase(ScrumrContext context, TEntity entity = null)
        {
            _view = new EditView(typeof(TEntity), context, (Entity)entity);

            _view.PostUpdated += (e) => PostUpdated(e);
            _view.PostCreated += (e) => PostCreated(e);
            _view.PreRendering += (r) => PreRendering(r);

            _view.Render();
        }

        protected abstract IEnumerable<Expression<Func<TEntity, object>>> OnRender();

        protected abstract void OnCreated(TEntity entity);

        protected virtual void OnUpdated(TEntity entity)
        {
            Context.SaveChanges();
        }

        public virtual ScrumrContext Context
        {
            get { return _view.Context; }
        }

        public virtual PropertyView GetView<T>()
        {
            return _view.PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as PropertyView;
        }

        public virtual TView GetView<T, TView>() where TView : class
        {
            return _view.PropertyViews.SingleOrDefault(x => x.Property.Type == typeof(T)) as TView;
        }

        private void PreRendering(Renderables r)
        {
            r.Items = OnRender().Select(x => x.GetExpressedProperty().Name);
        }

        private void PostCreated(Database.Entity entity)
        {
            OnCreated((TEntity)entity);
        }

        private void PostUpdated(Database.Entity entity)
        {
            OnUpdated((TEntity)entity);
        }

        public static EditEntityBase<TEntity> Create(ScrumrContext context, Entity entity = null, PropertyBag properties = null)
        {
            if (typeof(TEntity) == typeof(Feature))
                return new EditFeature(context, (Feature)entity, properties) as EditEntityBase<TEntity>;
            else if (typeof(TEntity) == typeof(Sprint))
                return new EditSprint(context, (Sprint)entity, properties) as EditEntityBase<TEntity>;
            else if (typeof(TEntity) == typeof(Project))
                return new EditProject(context, (Project)entity, properties) as EditEntityBase<TEntity>;
            else if (typeof(TEntity) == typeof(Ticket))
                return new EditTicket(context, (Ticket)entity, properties) as EditEntityBase<TEntity>;
            else
                throw new NotSupportedException("EditEntityBase<TEntity>.Create does not support this type");
        }

        public TEntity GetResult()
        {
            var result = _view.ShowDialog();

            if (!result.GetValueOrDefault())
                return default(TEntity);

            return (TEntity)_view.Result;
        }
    }
}
