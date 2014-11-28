using Scrumr.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scrumr.Client
{
    partial class HeaderView : UserControl
    {
        public event Action<Entity> RequestEdit;
        public event Action<Entity> RequestRemove;

        public Entity Entity { get; private set; }

        public HeaderView(dynamic entity, Orientation orientation)
        {
            InitializeComponent();

            labelHeader.Content = entity.Name;
            Entity = entity;

            if (orientation == Orientation.Horizontal)
                borderHeader.BorderThickness = new Thickness(0, 0, .5, 0);
            else if (orientation == Orientation.Vertical)
                borderHeader.BorderThickness = new Thickness(0, 0, 0, .5);

            FontWeight = FontWeights.Bold;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Edit", () => RequestEdit(Entity)));
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Remove", () => RequestRemove(Entity)));
        }
    }
}
