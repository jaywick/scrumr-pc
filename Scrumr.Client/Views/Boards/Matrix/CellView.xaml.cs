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
    partial class CellView : UserControl
    {
        public event Action<long, long> RequestNewTicket;

        public long SprintId { get; private set; }
        public long FeatureId { get; private set; }

        public CellView(long sprintId, long featureId)
        {
            InitializeComponent();

            SprintId = sprintId;
            FeatureId = featureId;

            AllowDrop = true;
            Background = Brushes.Transparent;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("New Ticket", () => RequestNewTicket(SprintId, FeatureId)));
        }

        internal void Add(TicketView ticketView)
        {
            listContents.Items.Add(ticketView);
        }
    }
}
