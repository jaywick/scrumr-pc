using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scrumr
{
    class CellView : ListBox
    {
        public event Action<int, int> RequestNewTicket;

        public int SprintId { get; private set; }
        public int FeatureId { get; private set; }

        public CellView(int sprintId, int featureId)
        {
            SprintId = sprintId;
            FeatureId = featureId;

            AllowDrop = true;
            Background = Brushes.Transparent;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewHelper.CreateMenuItem("New Ticket", () => RequestNewTicket(SprintId, FeatureId)));
        }
    }
}
