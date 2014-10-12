using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr
{
    class HeaderView : Label
    {
        public event Action<Entity> RequestEdit;
        public event Action<Entity> RequestRemove;

        public Entity Entity { get; private set; }

        public HeaderView(Entity entity)
        {
            Content = entity.Name;
            Entity = entity;

            FontWeight = FontWeights.Bold;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(Common.CreateMenuItem("Edit", () => RequestEdit(Entity)));
            ContextMenu.Items.Add(Common.CreateMenuItem("Remove", () => RequestRemove(Entity)));
        }
    }
}
