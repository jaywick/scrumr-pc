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

    }
}
