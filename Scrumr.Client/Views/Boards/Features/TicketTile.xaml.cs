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
    partial class TicketTile : UserControl, ITicketView
    {
        public event Action<Ticket> RequestClose;
        public event Action<Ticket> RequestReopen;
        public event Action<Ticket> RequestEdit;
        public event Action<Ticket> RequestRemove;

        public Ticket Project { get; set; }

        public TicketTile(Ticket ticket)
        {
            InitializeComponent();

            Project = ticket;

            labelName.Text = ticket.Name.ToString();
            labelName.Foreground = Brushes.White;

            if (ticket.State == TicketState.Closed)
            {
                textContainer.Background = CreateBrush("#99b433");
            }
            else if (ticket.IsBacklogged)
            {
                textContainer.Background = CreateBrush("#ffc40d");
            }
            else
            {
                textContainer.Background = CreateBrush("#2d89ef");
            }

            ContextMenu = new ContextMenu();

            if (Project.IsOpen)
                ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Close", () => RequestClose(ticket)));
            else
                ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Reopen", () => RequestReopen(ticket)));

            ContextMenu.Items.Add(new Separator());
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Edit", () => RequestEdit(ticket)));
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Remove", () => RequestRemove(ticket)));

            PreviewMouseMove += UserControl_PreviewMouseMove;
            PreviewMouseDoubleClick += (s, e) => RequestEdit(ticket);
        }

        void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var ticketView = (ITicketView)sender;
                var dependancyProperty = (DependencyObject)sender;
                DragDrop.DoDragDrop(dependancyProperty, new DataObject(typeof(Ticket), ticketView.Project), DragDropEffects.Move);
            }
        }

        private Brush CreateBrush(string hexCode)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexCode));
        }
    }
}
