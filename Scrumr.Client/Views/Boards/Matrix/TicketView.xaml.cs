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
    partial class TicketView : UserControl, ITicketView
    {
        public event Action<Ticket> RequestClose;
        public event Action<Ticket> RequestReopen; 
        public event Action<Ticket> RequestEdit;
        public event Action<Ticket> RequestRemove;

        public Ticket Project { get; set; }

        public TicketView(Ticket ticket)
        {
            InitializeComponent();

            Project = ticket;

            labelId.Text = "#" + ticket.ProjectTicketId.ToString();
            labelName.Text = ticket.Name.ToString();
            labelName.Foreground = ticket.State == TicketState.Open ? Brushes.Black : Brushes.Gray;
            borderId.Background = ticket.Type == TicketType.Task ? Brushes.LightBlue : Brushes.LightPink;

            ContextMenu = new ContextMenu();

            if (Project.IsOpen)
                ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Close", () => RequestClose(ticket)));
            else
                ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Reopen", () => RequestReopen(ticket)));

            ContextMenu.Items.Add(new Separator());
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Edit",  () => RequestEdit(ticket)));
            ContextMenu.Items.Add(ViewDirector.CreateMenuItem("Remove", () => RequestRemove(ticket)));

            PreviewMouseMove += UserControl_PreviewMouseMove;
        }

        void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var ticketView = (ITicketView)sender;
                var dependencyObject = (DependencyObject)sender;
                DragDrop.DoDragDrop(dependencyObject, new DataObject(typeof(Ticket), ticketView.Project), DragDropEffects.Move);
            }
        }
    }
}
