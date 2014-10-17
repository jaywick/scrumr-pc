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

namespace Scrumr
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => load();
        }

        ~MainWindow()
        {
            save();
        }

        private void load()
        {
            using (var context = new ScrumrContext())
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;

                var project = new Project();
                project.Name = "Project 2";
                var k = context.TicketTypes;
                context.Project.Add(project);
                context.SaveChanges();
            }

            /*Board.Context = 
            Board.Project = Board.Context.Projects.First(); // debug: get first project in list

            Board.Update();*/
        }

        private void save()
        {
            //Library.Save(Board.Context);
        }

    }
}
