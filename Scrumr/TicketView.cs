using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Scrumr
{
    class TicketView : ListBoxItem
    {
        public event Action<Ticket> RequestEdit;
        public event Action<Ticket> RequestRemove;

        public Ticket Ticket { get; private set; }

        public TicketView(Ticket ticket)
        {
            Ticket = ticket;

            Content = ticket.Name;
            
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(Common.CreateMenuItem("Edit",  () => RequestEdit(ticket)));
            ContextMenu.Items.Add(Common.CreateMenuItem("Remove", () => RequestRemove(ticket)));

            PreviewMouseMove += UserControl_PreviewMouseMove;
        }

        void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var thisControl = sender as TicketView;
                DragDrop.DoDragDrop(thisControl, new DataObject(typeof(Ticket), thisControl.Ticket), DragDropEffects.Move);
            }
        }
    }
}
