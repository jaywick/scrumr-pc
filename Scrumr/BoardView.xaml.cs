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
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        public List<Sprint> Sprints { get; set; }
        public List<Feature> Features { get; set; }
        public List<Ticket> Tickets { get; set; }

        public Dictionary<Sprint, int> SprintToColumnMap;
        public Dictionary<Feature, int> FeatureToRowMap;
        public Dictionary<ListBox, SprintFeature> CellMap;

        public BoardView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            SprintToColumnMap = new Dictionary<Sprint, int>();
            FeatureToRowMap = new Dictionary<Feature, int>();
            CellMap = new Dictionary<ListBox, SprintFeature>();

            Board.Children.Clear();

            CreateSprintColumns();
            CreateFeatureRows();
            CreateTicketCells();
        }

        private void CreateTicketCells()
        {
            for (int column = 0; column < Sprints.Count; column++)
            {
                for (int row = 0; row < Features.Count; row++)
                {
                    CreateTicketCell(column, row);
                }
            }
        }

        private void CreateFeatureRows()
        {
            int j = 0;
            Board.RowDefinitions.Clear();
            Board.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            foreach (var feature in Features)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                FeatureToRowMap.Add(feature, j);

                var featureLabel = new Label { Content = Features[j].Name, FontWeight = FontWeights.Bold };
                Board.Children.Add(featureLabel);
                Grid.SetColumn(featureLabel, 0);
                Grid.SetRow(featureLabel, j + 1);

                j++;
            }
        }

        private void CreateSprintColumns()
        {
            int i = 0;
            Board.ColumnDefinitions.Clear();
            Board.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            foreach (var sprint in Sprints)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                SprintToColumnMap.Add(sprint, i);

                var sprintLabel = new Label { Content = Sprints[i].Name, FontWeight = FontWeights.Bold };
                Board.Children.Add(sprintLabel);
                Grid.SetColumn(sprintLabel, i + 1);
                Grid.SetRow(sprintLabel, 0);

                i++;
            }
        }

        private void CreateTicketCell(int column, int row)
        {
            var sprint = Sprints[column];
            var feature = Features[row];

            var newCell = new ListBox
            {
                AllowDrop = true,
                Background = Brushes.Transparent,
            };
            newCell.Drop += newCell_Drop;

            CellMap.Add(newCell, new SprintFeature(column, row));
            Board.Children.Add(newCell);
            Grid.SetColumn(newCell, column + 1);
            Grid.SetRow(newCell, row + 1);

            var tickets = Tickets.Where(t => t.SprintId == sprint.ID && t.FeatureId == feature.ID);
            foreach (var ticket in tickets)
            {
                var newItem = new ListBoxItem { Content = ticket.Name };
                newItem.PreviewMouseMove += newItem_PreviewMouseMove;
                newItem.Tag = ticket;
                newCell.Items.Add(newItem);
            }
        }

        #region event handlers

        void newItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var thisControl = sender as ListBoxItem;
                DragDrop.DoDragDrop(thisControl, new DataObject(typeof(Ticket), thisControl.Tag as Ticket), DragDropEffects.Move);
            }
        }

        void newCell_Drop(object sender, DragEventArgs e)
        {
            var cell = sender as ListBox;
            var targetSprintFeature = CellMap[cell];
            var ticket = e.Data.GetData(typeof(Ticket)) as Ticket;

            ticket.SprintId = targetSprintFeature.Sprint;
            ticket.FeatureId = targetSprintFeature.Feature;

            Render();
        }

        #endregion
    }
}
