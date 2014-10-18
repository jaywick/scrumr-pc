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

namespace Scrumr
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => Load();
            this.Closing += (s, e) => Save();
        }

        private async void Load()
        {
            Board.Context = new ScrumrContext();
            Board.Context.LoadAll();

            MainMenu.IsEnabled = false;
            Board.Project = await GetDefaultProject();
            MainMenu.IsEnabled = true;

            Board.Update();
        }

        private void Save()
        {
            Board.Context.SaveChanges();
        }

        private void NewSprint(object sender, RoutedEventArgs e)
        {
            Add<Sprint>(Board.Context.Sprints);
        }

        private void NewFeature(object sender, RoutedEventArgs e)
        {
            Add<Feature>(Board.Context.Features);
        }

        private void NewTicket(object sender, RoutedEventArgs e)
        {
            Add<Ticket>(Board.Context.Tickets);
        }

        private void Add<T>(DbSet<T> table) where T : Entity
        {
            var propertiesView = new PropertiesView(typeof(T), Board.Context);

            if (propertiesView.ShowDialog() == true)
            {
                table.Add(propertiesView.Result as T);
            }

            Save();
            Board.Update();
        }
        
        private async Task<Project> GetDefaultProject()
        {
            return await Task.Run(() => Board.Context.Projects.First());
        }
    }
}
