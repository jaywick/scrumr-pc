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

            _view.PostUpdated += async (e) => await PostUpdated(e);
            _view.PostCreated += async (e) => await PostCreated(e);
            _view.PreDeleting += async (r) => await PreDeleting(r);
            _view.PreRendering += (r) => PreRendering(r);

            _view.Render();
        }

        protected abstract IEnumerable<Expression<Func<TEntity, object>>> OnRender();

        protected virtual async Task OnCreated(TEntity entity) { }

        protected virtual async Task OnUpdated(TEntity entity)
        {
            await Context.SaveChangesAsync();
        }

        protected virtual async Task OnDeleting(TEntity entity)
        {
            throw new InvalidOperationException(String.Format("OnDelete for {0} must be overriden", typeof(TEntity).Name));
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

        private async Task PostCreated(Database.Entity entity)
        {
            await OnCreated((TEntity)entity);
        }

        private async Task PostUpdated(Database.Entity entity)
        {
            await OnUpdated((TEntity)entity);
        }

        private async Task PreDeleting(Database.Entity entity)
        {
            await OnDeleting((TEntity)entity);
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

        public void Show()
        {
            _view.ShowDialog();
        }
    }
}
