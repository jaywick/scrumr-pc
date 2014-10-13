using System;
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

namespace Scrumr
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            load();
        }

        ~MainWindow()
        {
            save();
        }

        private void load()
        {
            Board.Context = Library.Load();
            Board.Project = Board.Context.Projects.First(); // debug: get first project in list

            Board.Update();
        }

        private void save()
        {
            Library.Save(Board.Context);
        }

        private void MenuNewSprint_Click(object sender, RoutedEventArgs e)
        {
            add(Board.Context.Sprints);
        }

        private void MenuNewFeature_Click(object sender, RoutedEventArgs e)
        {
            add(Board.Context.Features);
        }

        private void MenuNewTicket_Click(object sender, RoutedEventArgs e)
        {
            add(Board.Context.Tickets);
        }

        private void add<T>(List<T> list) where T : Entity
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
