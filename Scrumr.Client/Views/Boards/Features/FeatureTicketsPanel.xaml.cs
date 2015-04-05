﻿using Scrumr.Database;
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
    public partial class FeatureTicketsPanel : UserControl
    {
        public event Action Updated;

        public Feature Feature { get; set; }
        public Sprint Sprint { get; set; }

        private ScrumrContext Context { get; set; }

        public FeatureTicketsPanel()
        {
            InitializeComponent();
        }

        public FeatureTicketsPanel(ScrumrContext context, Feature feature, Sprint sprint)
            : this()
        {
            Context = context;
            Feature = feature;
            Sprint = sprint;

            var orderedTickets = Feature.Tickets
                .Where(MatchSprint)
                .OrderBy(x => !x.IsBacklogged)
                .ThenByDescending(x => x.State)
                .ThenBy(x => x.ID);

            foreach (var ticket in orderedTickets)
            {
                var ticketView = new TicketTile(ticket);

                ticketView.RequestClose += (t) => CloseTicket(t as Ticket);
                ticketView.RequestReopen += (t) => OpenTicket(t as Ticket);
                ticketView.RequestEdit += (t) => EditTicket(t as Ticket);
                ticketView.RequestRemove += async (t) => await RemoveEntity(t as Ticket);

                LayoutRoot.Children.Add(ticketView);
            }

            var addTile = new AddTicketTile(Feature, sprint);
            addTile.Added += AddedTicket;
            LayoutRoot.Children.Add(addTile);
        }

        private bool MatchSprint(Ticket ticket)
        {
            if (Sprint == null)
                return true;

            return ticket.SprintId == Sprint.ID;
        }

        void FeaturePanel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var zoomAmount = Math.Abs(e.DeltaManipulation.Scale.Length - Math.Sqrt(2));

            if ((TouchesOver.Count() == 2))
            {
                if (Math.Abs(zoomAmount - 0) > 0.1)
                {
                    ResizeTickets(-zoomAmount);
                }
            }

            e.Handled = true;
            base.OnManipulationDelta(e);
        }

        private void ResizeTickets(double factor)
        {
            foreach (var ticketView in LayoutRoot.Children.OfType<TicketTile>())
            {
                ticketView.Width = factor * 139;
                ticketView.Height = factor * 84;
            }
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

        private async Task RemoveEntity<T>(T entity) where T : Entity
        {
            await ViewDirector.RemoveEntity(entity, Context);
            Updated();
        }

        public async void AddedTicket(Ticket ticket)
        {
            Context.Tickets.Insert(ticket);
            await Context.SaveChangesAsync();

            Updated();
        }

        public void SetVisiblity(bool isVisible)
        {
            if (isVisible)
                this.Visibility = System.Windows.Visibility.Visible;
            else
                this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}