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
    public partial class FeaturePanel : UserControl
    {
        public event Action Updated;

        public Feature Feature { get; set; }

        private ScrumrContext Context { get; set; }

        public FeaturePanel()
        {
            InitializeComponent();
        }

        public FeaturePanel(ScrumrContext context, Feature feature)
            : this()
        {
            Context = context;
            Feature = feature;

            foreach (var ticket in Feature.GetTickets(Context).OrderBy(x => x.SprintId))
            {
                var ticketView = new TileTicketView(ticket);

                ticketView.RequestClose += (t) => CloseTicket(t as Ticket);
                ticketView.RequestReopen += (t) => OpenTicket(t as Ticket);
                ticketView.RequestEdit += (t) => EditTicket(t as Ticket);
                ticketView.RequestRemove += (t) => RemoveEntity(t as Ticket);

                LayoutRoot.Children.Add(ticketView);
            }

            var addTile = new AddButtonTileView(Feature, Feature.Project.Backlog);
            addTile.Added += AddedTicket;
            LayoutRoot.Children.Add(addTile);
        }

        private void OpenTicket(Ticket ticket)
        {
            ticket.Open();
            Updated();
        }

        private void CloseTicket(Ticket ticket)
        {
            ticket.Close();
            Updated();
        }

        private void EditTicket(Ticket ticket)
        {
            ViewDirector.EditTicket(ticket, Context);
            Updated();
        }

        private void RemoveEntity<T>(T entity) where T : Entity
        {
            ViewDirector.RemoveEntity(entity, Context);
            Updated();
        }

        public async void AddedTicket(Ticket ticket)
        {
            Context.Tickets.Add(ticket);
            await Context.SaveChangesAsync();

            Updated();
        }

        public void ToggleVisiblity()
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
                this.Visibility = System.Windows.Visibility.Collapsed;
            else
                this.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
