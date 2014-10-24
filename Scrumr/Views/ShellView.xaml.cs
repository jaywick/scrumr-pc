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

            this.Loaded += (s, e) => Load();
            this.Closing += (s, e) => Save();
        }

        private async void Load()
        {
            using (BusyDisplay)
            {
                Board.Context = new ScrumrContext();

                await Board.Context.LoadAllAsync();
                Board.Project = await GetDefaultProject();
                Board.Update();
            }
        }

        private void Save()
        {
            Board.Context.SaveChanges();
        }

        private async Task<Project> GetDefaultProject()
        {
            return await Task.Run(() => Board.Context.Projects.First());
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
