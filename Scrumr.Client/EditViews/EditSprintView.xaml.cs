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
    public partial class EditSprintView : MetroWindow
    {
        public Sprint Sprint { get; set; }
        public ScrumrContext Context { get; set; }

        public Modes Mode { get; private set; }

        int _projectId;

        public EditSprintView(ScrumrContext context, int projectId)
        {
            Mode = Modes.Creating;
            InitializeComponent();

            Context = context;
            _projectId = projectId;
        }

        public EditSprintView(ScrumrContext context, Sprint sprint)
        {
            Mode = Modes.Updating;
            InitializeComponent();

            Context = context;
            Sprint = sprint;

            SprintName.Text = sprint.Name;
            ProjectList.ItemsSource = Context.Projects;
            ProjectList.SelectedItem = sprint.Project;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == Modes.Creating)
            {
                Context.Sprints.Insert(Sprint);
            }
            else
            {
                Sprint.Name = SprintName.Text;
                Sprint.ProjectId = ((Project)ProjectList.SelectedItem).ID;
            }

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

            Context.Sprints.Remove(Sprint);

            Cancel_Click(sender, e);
        }
    }
}
