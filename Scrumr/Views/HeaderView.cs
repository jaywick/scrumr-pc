﻿using System;
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

        public HeaderView(dynamic entity)
        {
            Content = entity.Name;
            Entity = entity;

            FontWeight = FontWeights.Bold;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewHelper.CreateMenuItem("Edit", () => RequestEdit(Entity)));
            ContextMenu.Items.Add(ViewHelper.CreateMenuItem("Remove", () => RequestRemove(Entity)));
        }
    }
}
