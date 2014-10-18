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
            this.Loaded += (s, e) => load();
            this.Closing += (s, e) => save();
        }

        private async void load()
        {
            Board.Context = new ScrumrContext();
            Board.Project = await Task.Run(() => Board.Context.Projects.First());

            Board.Update();
        }

        private void save()
        {
            Board.Context.SaveChanges();
        }

        private void NewSprint(object sender, RoutedEventArgs e)
        {
            add(Board.Context.Sprints);
        }

        private void NewFeature(object sender, RoutedEventArgs e)
        {
            add(Board.Context.Features);
        }

        private void NewTicket(object sender, RoutedEventArgs e)
        {
            add(Board.Context.Tickets);
        }

        private void add<T>(DbSet<T> list) where T : Entity
        {
            var propertiesView = new PropertiesView(typeof(T), Board.Context);

            if (propertiesView.ShowDialog() == true)
            {
                list.Add(propertiesView.Result as T);
            }
            
            Board.Update();
        }


    }
}
