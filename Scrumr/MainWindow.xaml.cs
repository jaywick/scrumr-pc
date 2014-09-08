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
        public List<Sprint> Sprints { get; set; }
        public List<Feature> Features { get; set; }
        public List<Ticket> Tickets { get; set; }

        public Dictionary<Sprint, int> SprintToColumnMap;
        public Dictionary<Feature, int> FeatureToRowMap;
        public Dictionary<WrapPanel, Tuple<int, int>> CellMap;

        public MainWindow()
        {
            InitializeComponent();
            populateData();
            drawBoards();
        }

        private void populateData()
        {
            Features = new List<Feature>
            {
                new Feature { Name = "features 1" },
                new Feature { Name = "features 2" },
                new Feature { Name = "features 3" },
            };

            Sprints = new List<Sprint>
            {
                new Sprint { Name = "sprint 1" },
                new Sprint { Name = "sprint 2" },
                new Sprint { Name = "sprint 3" },
            };

            Tickets = new List<Ticket>
            {
                new Ticket { Name = "Ticket 1", Sprint = Sprints[0], Feature = Features[0] },
                new Ticket { Name = "Ticket 2", Sprint = Sprints[1], Feature = Features[0] },
                new Ticket { Name = "Ticket 3", Sprint = Sprints[2], Feature = Features[2] },
                new Ticket { Name = "Ticket 4", Sprint = Sprints[1], Feature = Features[1] },
                new Ticket { Name = "Ticket 5", Sprint = Sprints[1], Feature = Features[1] },
            };
        }

        private void drawBoards()
        {
            int i = 0, j = 0;
            SprintToColumnMap = new Dictionary<Sprint, int>();
            FeatureToRowMap = new Dictionary<Feature, int>();
            CellMap = new Dictionary<WrapPanel, Tuple<int,int>>();

            foreach (var sprint in Sprints)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                SprintToColumnMap.Add(sprint, i++);
            }

            foreach (var feature in Features)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                FeatureToRowMap.Add(feature, j++);
            }

            for (int column = 0; column < Sprints.Count; column++)
            {
                for (int row = 0; row < Features.Count; row++)
                {
                    var sprint = Sprints[column];
                    var feature = Features[row];

                    var newCell = new WrapPanel();

                    CellMap.Add(newCell, Tuple.Create(column,row));
                    Board.Children.Add(newCell);
                    Grid.SetColumn(newCell, column);
                    Grid.SetRow(newCell, row);

                    var tickets = Tickets.Where(t => t.Sprint == sprint && t.Feature == feature);
                    foreach (var ticket in tickets)
                    {
                        var newTicket = new Button { Content = ticket.Name, Height = 20, Width = 200 };
                        newCell.Children.Add(newTicket);
                    }
                }
            }
        }
    }
}
