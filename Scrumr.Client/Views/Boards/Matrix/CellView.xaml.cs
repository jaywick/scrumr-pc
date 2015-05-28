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
        public event Action<Guid> RequestNewTicket;

        public Guid FeatureId { get; private set; }

        public CellView(Guid featureId)
        {
            InitializeComponent();

            FeatureId = featureId;

            AllowDrop = true;
            Background = Brushes.Transparent;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("New Ticket", () => RequestNewTicket(FeatureId)));
        }

        internal void Add(TicketView ticketView)
        {
            listContents.Items.Add(ticketView);
        }
    }
}
