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
using MahApps.Metro.Controls.Dialogs;
using Scrumr.Client.Views;

namespace Scrumr.Client
{
    public partial class MainWindow : MetroWindow
    {
        private bool _lockProjectSelection = false;
        private bool _lockSaving = false;
        private ContextMenu _addMenu;
        private ContextMenu _projectsList;
        private ShortcutMaps _shortcuts = new ShortcutMaps();

        private string SourceFile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.LeftWindowCommands = new WindowCommands();

            this.ProjectsList.Items.Clear();
            this.Loaded += async (s, e) => await LoadAsync();
            this.Closing += async (s, e) => await SaveAsync();

            loadCommands();
            loadShortcuts();
        }

        private void loadShortcuts()
        {
            _shortcuts.Add(ModifierKeys.Control, Key.N, () => _addMenu.IsOpen = true);
            _shortcuts.Add(ModifierKeys.Control, Key.T, () => Board.NewTicket());
            _shortcuts.Add(ModifierKeys.Control, Key.S, async () => await Save());
            _shortcuts.Add(ModifierKeys.Control, Key.O, () => ChooseFile());
        }

        private async Task Save()
        {
            savedDisplay.FadeIn(0.1);
            
            await SaveAsync();
            await Task.Delay(TimeSpan.FromSeconds(3));

            savedDisplay.FadeOut(1);
        }

        private void loadCommands()
        {
            _addMenu = new ContextMenu();
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Ticket", Board.NewTicket));
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Feature", Board.NewFeature));
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Sprint", Board.NewSprint));
            _addMenu.Items.Add(ViewDirector.CreateMenuItem("Project", Board.NewProject));
            _addMenu.PreviewKeyDown += (s, e) => ProcessAddMenuShortcut(e.Key);

            AddButton.Click += (s, e) => _addMenu.IsOpen = true;
            _addMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _addMenu.PlacementTarget = AddButton;

            _projectsList = new ContextMenu();
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Configure project", EditProject));
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Choose database", ChooseFile));
            _projectsList.Items.Add(ViewDirector.CreateMenuItem("Create new database", CreateFile));

            ManageProjectsButton.Click += (s, e) => _projectsList.IsOpen = true;
            _projectsList.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _projectsList.PlacementTarget = ManageProjectsButton;
        }

        private async Task LoadAsync()
        {
            using (BusyDisplay)
            {
                SourceFile = App.Preferences[Preferences.SourceFileKey] ?? "scrumr.sqlite";
                Board.Context = FileSystem.LoadContext(SourceFile, App.SchemaVersion);

                Task errorMessageTask = null;
                try
                {
                    await Board.Context.LoadAllAsync();
                }
                catch (SchemaMismatchException ex)
                {
                    errorMessageTask = this.ShowMessageAsync("", String.Format(
                        "The database you are trying to laod is out of date and cannot be used with this version of the application.\n" +
                        "Application expects v{0}, however database is v{1}.\n\n" +
                        "{2}", ex.ExpectedVersion, ex.ActualVersion, ex.FilePath));
                }
                catch (Exception ex)
                {
                    errorMessageTask = this.ShowMessageAsync("Could not load database", ex.Message);
                }

                if (errorMessageTask != null)
                {
                    await errorMessageTask;
                    return;
                }

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
            App.Preferences[Preferences.DefaultProjectKey] = project.Name;
        }

        private void EditProject()
        {
            ViewDirector.EditEntity(Board.Project, Board.Context);
        }

        private async void ChooseFile()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".sqlite",
                Filter = "SQLite Database File (*.sqlite)|*.sqlite",
                CheckFileExists = true,
            };

            if (dialog.ShowDialog() == false)
                return;

            App.Preferences[Preferences.SourceFileKey] = dialog.FileName;
            await LoadAsync();
        }

        private async void CreateFile()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".sqlite",
                Filter = "SQLite Database File (*.sqlite)|*.sqlite",
                OverwritePrompt = true,
            };

            if (dialog.ShowDialog() == false)
                return;

            var path = dialog.FileName;

            await FileSystem.CreateNew(path, App.SchemaVersion);

            App.Preferences[Preferences.SourceFileKey] = path;
            await LoadAsync();
        }

        private async Task SaveAsync()
        {
            await Board.Context.SaveChangesAsync();
        }

        private async Task<Project> GetDefaultProjectAsync()
        {
            var defaultProject = App.Preferences[Preferences.DefaultProjectKey];

            if (defaultProject == null)
                return await Board.Context.Projects.FirstAsync();

            var project = await Board.Context.Projects.SingleOrDefaultAsync(x => x.Name == defaultProject);

            if (project == null)
                return await Board.Context.Projects.FirstAsync();

            return project;
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

        #region Shortcuts

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _shortcuts.Process(Keyboard.Modifiers, e.Key);
        }

        private void ProcessAddMenuShortcut(Key key)
        {
            switch (key)
            {
                case Key.T: Board.NewTicket();break;
                case Key.S: Board.NewSprint(); break;
                case Key.F: Board.NewFeature(); break;
                case Key.P: Board.NewProject(); break;
                default: break;
            }
        }

        #endregion

    }
}
