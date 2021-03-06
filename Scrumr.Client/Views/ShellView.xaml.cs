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
using System.Runtime;
using MahApps.Metro.Controls;
using Scrumr.Database;
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;
using Scrumr.Client.Views;
using Scrumr.Core;

namespace Scrumr.Client
{
    public partial class MainWindow : MetroWindow
    {
        private ShortcutMaps _shortcuts = new ShortcutMaps();
        private bool _isShuttingDown = false;

        private string SourceFile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.LeftWindowCommands = new WindowCommands();
            this.BoardControl.Content = new MainBoard();

            this.Loaded += async (s, e) => await LoadAsync();
            this.Closing += async (s, e) => { e.Cancel = true; await SaveAndExitAsync(); };
        }


        public IBoardView Board
        {
            get { return (IBoardView)BoardControl.Content; }
            set { BoardControl.Content = value; }
        }

        private void LoadShortcuts()
        {
            _shortcuts.Add(ModifierKeys.Control, Key.T, () => NewTicket());
            _shortcuts.Add(ModifierKeys.Control, Key.S, async () => await Save());
            _shortcuts.Add(ModifierKeys.Control, Key.O, async () => await ChooseFileAsync());
        }

        private async Task Save()
        {
            savedDisplay.FadeIn(0.1);

            await SaveAsync();
            await Task.Delay(TimeSpan.FromSeconds(3));

            savedDisplay.FadeOut(1);
        }

        private void LoadCommands()
        {
            MenuButton.Click += (s, e) => MenuFlyout.IsOpen = !MenuFlyout.IsOpen;

            MenuFlyoutContent.RequestEditProject += () => { MenuFlyout.IsOpen = false; EditProject(); };
            MenuFlyoutContent.RequestChooseFile += async () => { MenuFlyout.IsOpen = false; await ChooseFileAsync(); };
            MenuFlyoutContent.RequestCreateFile += async () => { MenuFlyout.IsOpen = false; await CreateFileAsync(); };
            MenuFlyoutContent.RequestNewTicket += () => { MenuFlyout.IsOpen = false; NewTicket(); };
            MenuFlyoutContent.RequestNewFeature += () => { MenuFlyout.IsOpen = false; NewFeature(); };
            MenuFlyoutContent.RequestNewSprint += () => { MenuFlyout.IsOpen = false; NewSprint(); };
            MenuFlyoutContent.RequestNewProject += () => NewProject();
            MenuFlyoutContent.RequestShowHideClosedTickets += () => { MenuFlyout.IsOpen = false; ToggleClosedTicketsDisplay(); };
            MenuFlyoutContent.RequestShowHideEmptyFeatures += () => { MenuFlyout.IsOpen = false; ToggleEmptyFeaturesDisplay(); };

            MenuFlyoutContent.Load(Board.Context);
        }

        private async Task LoadAsync()
        {
            using (BusyDisplay)
            {
                SourceFile = App.Preferences[Preferences.SourceFileKey];

                HideBlank();
                if (String.IsNullOrWhiteSpace(SourceFile))
                {
                    ShowBlank("Welcome to Scrumr. You can load an existing database or create a new one.");
                    return;
                }

                if (!System.IO.File.Exists(SourceFile))
                {
                    Logger.Log("Expected database file missing: " + SourceFile);
                    ShowBlank("Database is missing. Perhaps it was moved, deleted or renamed?\nExepected file at: " + SourceFile);
                    return;
                }

                try
                {
                    Board.Context = await FileSystem.LoadContext(SourceFile, Meta.CurrentSchemaVersion);
                }
                catch (SchemaMismatchException ex)
                {
                    Logger.Log("ERROR: " + ex.Message);
                    ShowBlank(String.Format(
                        "The database you are trying to load is out of date and cannot be used with this version of the application.\n" +
                        "Application expects v{0}, however database is v{1}.\n\n" +
                        "{2}", ex.ExpectedVersion, ex.ActualVersion, ex.FilePath));
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR: " + ex.Message);
                    ShowBlank("Could not load database\n" + ex.Message);
                    return;
                }

                Board.ShowClosedTickets = Boolean.Parse(App.Preferences[Preferences.ShowClosedTickets, "True"]);
                Board.ShowEmptyFeatures = Boolean.Parse(App.Preferences[Preferences.ShowEmptyFeatures, "True"]);
                Board.Project = GetDefaultProject();
                Board.Update();

                LoadCommands();
                LoadShortcuts();
            }
        }

        private void HideBlank()
        {
            BlankPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ShowBlank(string message)
        {
            BlankPanel.Visibility = System.Windows.Visibility.Visible;
            BlankPanel.Message = message;
        }

        private void SwitchProject(Project project)
        {
            Board.Project = project;
        }

        private void ToggleClosedTicketsDisplay()
        {
            Board.ShowClosedTickets = !Board.ShowClosedTickets;
            App.Preferences[Preferences.ShowClosedTickets] = Board.ShowClosedTickets.ToString();
            Board.Update();
        }

        private void ToggleEmptyFeaturesDisplay()
        {
            Board.ShowEmptyFeatures = !Board.ShowEmptyFeatures;
            App.Preferences[Preferences.ShowEmptyFeatures] = Board.ShowEmptyFeatures.ToString();
            Board.Update();
        }

        private void EditProject()
        {
            ViewDirector.EditEntity(Board.Project, Board.Context);

            if (Board.Project == null)
                return;
        }

        private async void ChooseFile()
        {
            await ChooseFileAsync();
        }

        private async Task ChooseFileAsync()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".scrumr",
                Filter = "Scrumr Database File (*.scrumr)|*.scrumr",
                CheckFileExists = true,
            };

            if (dialog.ShowDialog() == false)
                return;

            App.Preferences[Preferences.SourceFileKey] = dialog.FileName;
            await LoadAsync();
        }

        private async void CreateFile()
        {
            await CreateFileAsync();
        }

        private async Task CreateFileAsync()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".scrumr",
                Filter = "Scrumr Database File (*.scrumr)|*.scrumr",
                OverwritePrompt = true,
            };

            if (dialog.ShowDialog() == false)
                return;

            var path = dialog.FileName;

            await FileSystem.CreateNew(path, Meta.CurrentSchemaVersion);

            App.Preferences[Preferences.SourceFileKey] = path;
            await LoadAsync();
        }

        private async Task SaveAndExitAsync()
        {
            if (_isShuttingDown)
                return;

            _isShuttingDown = true;

            if (Board.Context != null)
            {
                await Board.Context.SaveChangesAsync();
            }
            else
            {
                Logger.Log("WARN: Board.Context == null while trying to save");
            }

            Application.Current.Shutdown();
        }

        private async Task SaveAsync()
        {
            await Board.Context.SaveChangesAsync();
        }

        private Project GetDefaultProject()
        {
            var defaultProject = App.Preferences[Preferences.DefaultProjectKey];

            if (defaultProject == null)
                return Board.Context.Projects.First();

            var project = Board.Context.Projects.SingleOrDefault(x => x.Name == defaultProject);

            if (project == null)
                return Board.Context.Projects.First();

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
                _mainWindow.BoardControl.Visibility = System.Windows.Visibility.Collapsed;
            }

            public void Dispose()
            {
                _mainWindow.ProgressBusy.Visibility = System.Windows.Visibility.Collapsed;
                _mainWindow.BoardControl.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _shortcuts.Process(Keyboard.Modifiers, e.Key);
        }
        public void NewSprint()
        {
            ViewDirector.AddEntity<Sprint>(Board.Context, Board.Project.ID);
            Board.Update();
        }

        public void NewFeature()
        {
            ViewDirector.AddEntity<Feature>(Board.Context, Board.Project.ID);
            Board.Update();
        }

        public void NewTicket()
        {
            ViewDirector.AddTicket(Board.Context, Board.Project.ID);
            Board.Update();
        }

        public void NewProject()
        {
            var project = ViewDirector.AddEntity<Project>(Board.Context);

            if (project == null)
                return;
        }

        private void MenuFlyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            if (MenuFlyout.IsOpen)
                FlyoutOverlay.FadeIn(0.5);
            else
                FlyoutOverlay.FadeOut(0.5);
        }

        private void FlyoutOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MenuFlyout.IsOpen = false;
        }
    }
}
