using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
using System.Runtime;
using System.Data.Entity;
using MahApps.Metro.Controls;
using Scrumr.Database;
using Microsoft.Win32;

namespace Scrumr.Client
{
    public partial class MainWindow : MetroWindow
    {
        private bool _lockProjectSelection = false;

        private string SourceFile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.LeftWindowCommands = new WindowCommands();

            this.Loaded += (s, e) => Load();
            this.Closing += async (s, e) => await Save();

            loadCommands();
            this.ProjectsList.Items.Clear();
        }

        private void loadCommands()
        {
            var addMenu = new ContextMenu();
            addMenu.Items.Add(ViewDirector.CreateMenuItem("Ticket", Board.NewTicket));
            addMenu.Items.Add(ViewDirector.CreateMenuItem("Feature", Board.NewFeature));
            addMenu.Items.Add(ViewDirector.CreateMenuItem("Sprint", Board.NewSprint));
            addMenu.Items.Add(ViewDirector.CreateMenuItem("Project", Board.NewProject));

            AddButton.Click += (s, e) => addMenu.IsOpen = true;
            addMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            addMenu.PlacementTarget = AddButton;

            var manageProjectMenu = new ContextMenu();
            manageProjectMenu.Items.Add(ViewDirector.CreateMenuItem("Configure Project", EditProject));
            manageProjectMenu.Items.Add(ViewDirector.CreateMenuItem("Choose File", ChooseFile));

            ManageProjectsButton.Click += (s, e) => manageProjectMenu.IsOpen = true;
            manageProjectMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            manageProjectMenu.PlacementTarget = ManageProjectsButton;
        }

        private async void Load()
        {
            using (BusyDisplay)
            {
                SourceFile = App.Preferences[Preferences.SourceFileKey] ?? "scrumr.sqlite";
                Board.Context = await FileSystem.LoadContext(SourceFile, App.Overwrite);

                await Board.Context.LoadAllAsync();
                Board.Project = await GetDefaultProjectAsync();
                this.ProjectsList.SelectedItem = Board.Project;
                Board.Update();

                ReloadProjectsList();
            }
        }

        private void ReloadProjectsList()
        {
            ProjectsList.Items.Clear();

            foreach (var item in Board.Context.Projects)
                ProjectsList.Items.Add(item);
        }

        private void OnProjectSelected(object s, SelectionChangedEventArgs e)
        {
            if (_lockProjectSelection)
                return;

            if (e.AddedItems.Count == 0)
                return;

            Project selectedProject;

            if (e.AddedItems.Count > 0)
                selectedProject = e.AddedItems[e.AddedItems.Count - 1] as Project;
            else
                selectedProject = e.AddedItems[0] as Project;

            if (selectedProject == null)
                return;

            SwitchProject(selectedProject);
        }

        private void OnProjectAdded(Project project)
        {
            _lockProjectSelection = true;
            ReloadProjectsList();
            _lockProjectSelection = false;

            ProjectsList.SelectedItem = project;
        }

        private void SwitchProject(Project project)
        {
            Board.Project = project;
        }

        private void EditProject()
        {
            ViewDirector.EditEntity(Board.Project, Board.Context);
        }

        private void ChooseFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".sqlite",
                Filter = "SQLite Database File (*.sqlite)|*.sqlite",
            };

            if (openFileDialog.ShowDialog() == false)
                return;

            App.Preferences[Preferences.SourceFileKey] = openFileDialog.FileName;
            Load();
        }

        private async Task Save()
        {
            await Board.Context.SaveChangesAsync();
        }

        private async Task<Project> GetDefaultProjectAsync()
        {
            return await Board.Context.Projects.FirstAsync();
        }

        #region BusyDisplay

        public DisposableBusyDisplay BusyDisplay
        {
            get { return new DisposableBusyDisplay(this); }
        }

        public class DisposableBusyDisplay : IDisposable
        {
            MainWindow _mainWindow;

            public DisposableBusyDisplay(MainWindow mainWindow)
            {
                _mainWindow = mainWindow;

                _mainWindow.ProgressBusy.Visibility = System.Windows.Visibility.Visible;
                _mainWindow.Board.Visibility = System.Windows.Visibility.Collapsed;
            }

            public void Dispose()
            {
                _mainWindow.ProgressBusy.Visibility = System.Windows.Visibility.Collapsed;
                _mainWindow.Board.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion
    }
}
