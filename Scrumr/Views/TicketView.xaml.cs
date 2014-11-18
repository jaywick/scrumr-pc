﻿using System;
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

namespace Scrumr
{
    partial class TicketView : UserControl
    {
        public event Action<Ticket> RequestEdit;
        public event Action<Ticket> RequestRemove;

        public Ticket Ticket { get; private set; }

        public TicketView(Ticket ticket)
        {
            InitializeComponent();

            Ticket = ticket;

            labelName.Content = string.Format("#{0}: {1}", ticket.ProjectTicketId, ticket.Name);

            Foreground = ticket.State == TicketState.Open ? Brushes.Black : Brushes.Gray;

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(ViewHelper.CreateMenuItem("Edit",  () => RequestEdit(ticket)));
            ContextMenu.Items.Add(ViewHelper.CreateMenuItem("Remove", () => RequestRemove(ticket)));

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