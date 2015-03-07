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

namespace Scrumr.Client
{
    public partial class MainWindow : MetroWindow
    {
        private ShortcutMaps _shortcuts = new ShortcutMaps();

        private string SourceFile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.LeftWindowCommands = new WindowCommands();
            this.BoardControl.Content = new FeatureView();

            this.Loaded += async (s, e) => await LoadAsync();
            this.Closing += async (s, e) => await SaveAsync();
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
            _shortcuts.Add(ModifierKeys.Control, Key.O, async () => await ChooseFile());
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
            
            MenuFlyoutContent.RequestEditProject += () => EditProject();
            MenuFlyoutContent.RequestChooseFile += async () => await ChooseFile();
            MenuFlyoutContent.RequestCreateFile += async () => await CreateFile();
            MenuFlyoutContent.RequestNewTicket += () => NewTicket();
            MenuFlyoutContent.RequestNewFeature += () => NewFeature();
            MenuFlyoutContent.RequestNewSprint += () => NewSprint();
            MenuFlyoutContent.RequestNewProject += () => NewProject();
            MenuFlyoutContent.RequestCloseFlyout += () => MenuFlyout.IsOpen = !MenuFlyout.IsOpen;
            MenuFlyoutContent.ProjectSelected += (p) => SwitchProject(p);

            MenuFlyoutContent.Load(Board.Context);
        }

        private async Task LoadAsync()
        {
            using (BusyDisplay)
            {
                SourceFile = App.Preferences[Preferences.SourceFileKey];

                if (String.IsNullOrWhiteSpace(SourceFile))
                {
                    await this.ShowMessageAsync("Welcome to Scrumr!", "Lets create your first database");
                    await CreateFile();
                    return;
                }
                if (!System.IO.File.Exists(SourceFile))
                {
                    await this.ShowMessageAsync("Database is missing", "Perhaps it was moved, deleted or renamed?\nExepected file at: " + SourceFile);
                    await ChooseFile();
                    return;
                }

                Task errorMessageTask = null;
                try
                {
                    Board.Context = await FileSystem.LoadContext(SourceFile, App.SchemaVersion);
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

                Board.Project = GetDefaultProject();
                MenuFlyoutContent.SelectProject(Board.Project);
                Board.Update();

                LoadCommands();
                LoadShortcuts();

                MenuFlyoutContent.Update();
            }
        }

        private void SwitchProject(Project project)
        {
            Board.Project = project;
            App.Preferences[Preferences.DefaultProjectKey] = project.Name;
        }

        private void EditProject()
        {
            new EditProjectView(Board.Context, Board.Project).ShowDialog();
        }

        private async Task ChooseFile()
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

        private async Task CreateFile()
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

            await FileSystem.CreateNew(path, App.SchemaVersion);
            
            App.Preferences[Preferences.SourceFileKey] = path;
            await LoadAsync();
        }

        private async Task SaveAsync()
        {
            if (Board.Context != null)
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
            new EditSprintView(Board.Context, Board.Project.ID).ShowDialog();
            Board.Update();
        }

        public void NewFeature()
        {
            new EditFeatureView(Board.Context, Board.Project.ID).ShowDialog();
            Board.Update();
        }

        public void NewTicket()
        {
            new EditTicketView(Board.Context, Board.Project.ID).ShowDialog();
            Board.Update();
        }

        public void NewProject()
        {
            new EditProjectView(Board.Context).ShowDialog();
            Board.Update();
        }
    }
}
