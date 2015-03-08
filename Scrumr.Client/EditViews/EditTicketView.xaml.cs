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

        public EditTicketView(ScrumrContext context, Modes mode)
        {
            InitializeComponent();
            Context = context;
            Mode = mode;
        }

        public static bool? Create(ScrumrContext context, int projectId)
        {
            var instance = new EditTicketView(context, Modes.Creating);
            instance._projectId = projectId;
            instance.Load();
            
            return instance.ShowDialog();
        }

        public static bool? Create(ScrumrContext context, int sprintId, int featureId)
        {
            var instance = new EditTicketView(context, Modes.Creating);
            instance._sprintId = sprintId;
            instance._featureId = featureId;
            instance.Load();

            return instance.ShowDialog();
        }

        public static bool? Edit(ScrumrContext context, Ticket ticket)
        {
            var instance = new EditTicketView(context, Modes.Creating);
            instance.Ticket = ticket;
            instance.Load();
            
            return instance.ShowDialog();
        }

        private void Load()
        {
            TicketName.Text = Ticket.Name;
            TicketDescription.Text = Ticket.Description;
            
            FeatureList.ItemsSource = Context.Features;
            FeatureList.SelectedItem = Ticket.Feature;
            
            SprintList.ItemsSource = Context.Sprints;
            SprintList.SelectedItem = Ticket.Sprint;
            
            TypeList.ItemsSource = Enum.GetValues(typeof(TicketType)).Cast<TicketType>();
            TypeList.SelectedItem = Ticket.Type;

            StateList.ItemsSource = Enum.GetValues(typeof(TicketState)).Cast<TicketState>();
            StateList.SelectedItem = Ticket.State;
        }

        private void Save()
        {
            Ticket.Name = TicketName.Text;
            Ticket.Description = TicketDescription.Text;
            Ticket.FeatureId = ((Feature)FeatureList.SelectedItem).ID;
            Ticket.SprintId = ((Sprint)SprintList.SelectedItem).ID;
            Ticket.Type = ((TicketType)TypeList.SelectedItem);
            Ticket.State = ((TicketState)StateList.SelectedItem);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == Modes.Creating)
                Context.Tickets.Insert(Ticket);
            else
                Save();

            await Context.SaveChangesAsync();
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
