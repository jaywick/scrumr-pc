using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel.DataAnnotations.Schema;
using Scrumr.Database;

namespace Scrumr.Client
{
    public partial class EditTicketView : MetroWindow
    {
        private int? _projectId;
        private int? _sprintId;
        private int? _featureId;

        public Ticket Ticket { get; set; }
        public ScrumrContext Context { get; set; }

        public Modes Mode { get; private set; }

        public EditTicketView(ScrumrContext context, int projectId)
        {
            Mode = Modes.Creating;
            InitializeComponent();

            Context = context;
            _projectId = projectId;
        }

        public EditTicketView(ScrumrContext context, int sprintId, int featureId)
        {
            Mode = Modes.Creating;
            InitializeComponent();

            Context = context;
            _sprintId = sprintId;
            _featureId = featureId;
        }

        public EditTicketView(ScrumrContext context, Ticket ticket)
        {
            Mode = Modes.Updating;
            InitializeComponent();

            Context = context;
            Ticket = ticket;

            TicketName.Text = ticket.Name;
            FeatureList.ItemsSource = Context.Features;
            FeatureList.SelectedItem = ticket.Feature;
            SprintList.ItemsSource = Context.Sprints;
            SprintList.SelectedItem = ticket.Sprint;
            //TypeList.ItemsSource = 
            //TypeList.SelectedItem = ticket.;
            //StateList.ItemsSource = 
            //StateList.SelectedItem = ticket.;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Context.Tickets.Insert(Ticket);

            DialogResult = true;
            Hide();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = await this.ShowMessageAsync
            (
                "Confirm Delete",
                "This will delete all items contained within. Are you sure you wish to continue?",
                MessageDialogStyle.AffirmativeAndNegative
            );

            if (confirmResult == MessageDialogResult.Negative)
                return;

            Context.Tickets.Remove(Ticket);

            Cancel_Click(sender, e);
        }
    }
}
