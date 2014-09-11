using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scrumr
{
	/// <summary>
	/// Interaction logic for TicketView.xaml
	/// </summary>
	public partial class TicketView : UserControl
	{
		public TicketView()
		{
			this.InitializeComponent();
		}

        public TicketView(Ticket ticket)
            : this()
        {
            this.Ticket = ticket;

            // TEMP: until we fix binding
            labelName.Content = Name;
        }

        public Ticket Ticket { get; set; }

        public string Name
        {
            get { return Ticket.Name; }
        }

        public int ID
        {
            get { return Ticket.ID; }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as TicketView, new DataObject(typeof(Ticket), Ticket), DragDropEffects.Move);
            }
        }
	}
}