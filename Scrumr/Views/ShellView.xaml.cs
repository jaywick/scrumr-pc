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

namespace Scrumr
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.LeftWindowCommands = new WindowCommands();
            
            this.Loaded += (s, e) => Load();
            this.Closing += (s, e) => Save();

            loadAddButton();
            setupProjectsList();
        }

        private void setupProjectsList()
        {
            this.ProjectsList.Items.Clear();
            this.ProjectsList.SelectionChanged += (s, e) => SwitchProject(e.AddedItems[0] as Project);
        }

        private void loadAddButton()
        {
            var _addMenu = new ContextMenu();
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("Ticket", Board.NewTicket));
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("Feature", Board.NewFeature));
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("Sprint", Board.NewSprint));
            _addMenu.Items.Add(ViewHelper.CreateMenuItem("Project", Board.NewProject));

            AddButton.Click += (s, e) => _addMenu.IsOpen = true;
            _addMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _addMenu.PlacementTarget = AddButton;
        }

        private async void Load()
        {
            using (BusyDisplay)
            {
                Board.Context = FileSystem.LoadContext();

                await Board.Context.LoadAllAsync();
                Board.Project = await GetDefaultProjectAsync();
                this.ProjectsList.SelectedItem = Board.Project;
                Board.Update();

                Board.Context.Projects.ToList().ForEach(x => this.ProjectsList.Items.Add(x));
            }
        }

        private void SwitchProject(Project project)
        {
            Board.Project = project;
        }

        private void Save()
        {
            Board.Context.SaveChanges();
        }

        private async Task<Project> GetDefaultProjectAsync()
        {
            return await Board.Context.Projects.FirstAsync();
        }

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
    }
}
