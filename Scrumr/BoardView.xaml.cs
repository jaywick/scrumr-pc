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

namespace Scrumr
{
    public partial class BoardView : UserControl
    {
        public List<Sprint> Sprints { get; set; }
        public List<Feature> Features { get; set; }
        public List<Ticket> Tickets { get; set; }

        public Dictionary<Sprint, int> SprintToColumnMap;
        public Dictionary<Feature, int> FeatureToRowMap;
        public Dictionary<ListBox, SprintFeature> CellMap;

        public Func<Sprint, bool> SprintFilter { get; set; }
        public Func<Feature, bool> FeatureFilter { get; set; }
        public Func<Ticket, bool> TicketFilter { get; set; }

        public IEnumerable<Sprint> VisibleSprints
        {
            get { return Sprints.Where(SprintFilter); }
        }

        public IEnumerable<Feature> VisibleFeatures
        {
            get { return Features.Where(FeatureFilter); }
        }

        public IEnumerable<Ticket> VisibleTickets
        {
            get { return Tickets.Where(TicketFilter); }
        }

        public BoardView()
        {
            InitializeComponent();

            SprintFilter = (x) => true;
            FeatureFilter = (x) => true;
            TicketFilter = (x) => true;
        }

        public void Update()
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
            for (int column = 0; column < VisibleSprints.Count(); column++)
            {
                for (int row = 0; row < VisibleFeatures.Count(); row++)
                {
                    CreateTicketCell(column, row);
                }
            }
        }

        private void CreateFeatureRows()
        {
            int i = 0;
            Board.RowDefinitions.Clear();
            Board.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            foreach (var feature in VisibleFeatures)
            {
                Board.RowDefinitions.Add(new RowDefinition());
                FeatureToRowMap.Add(feature, i);

                var featureLabel = new Label { Content = feature.Name, FontWeight = FontWeights.Bold };
                featureLabel.ContextMenu = new ContextMenu();
                featureLabel.ContextMenu.Items.Add(Common.CreateMenuItem("Edit", () => editEntity<Feature>(feature)));
                featureLabel.ContextMenu.Items.Add(Common.CreateMenuItem("Remove", () => removeEntity<Feature>(feature)));
                AddToGrid(featureLabel, 0, i + 1);

                i++;
            }
        }

        private void CreateSprintColumns()
        {
            int i = 0;
            Board.ColumnDefinitions.Clear();
            Board.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            foreach (var sprint in VisibleSprints)
            {
                Board.ColumnDefinitions.Add(new ColumnDefinition());
                SprintToColumnMap.Add(sprint, i);

                var sprintLabel = new Label { Content = sprint.Name, FontWeight = FontWeights.Bold };
                sprintLabel.ContextMenu = new ContextMenu();
                sprintLabel.ContextMenu.Items.Add(Common.CreateMenuItem("Edit", () => editEntity<Sprint>(sprint)));
                sprintLabel.ContextMenu.Items.Add(Common.CreateMenuItem("Remove", () => removeEntity<Sprint>(sprint)));
                AddToGrid(sprintLabel, i + 1, 0);

                i++;
            }
        }

        private void AddToGrid(UIElement item, int column, int row)
        {
            Board.Children.Add(item);

            Grid.SetColumn(item, column);
            Grid.SetRow(item, row);
        }

        private void CreateTicketCell(int sprintId, int featureId)
        {
            var newCell = new ListBox { AllowDrop = true, Background = Brushes.Transparent };
            CellMap.Add(newCell, new SprintFeature(sprintId, featureId));
            AddToGrid(newCell, sprintId + 1, featureId + 1);
            newCell.Drop += newCell_Drop;

            var tickets = VisibleTickets.Where(t => t.SprintId == sprintId)
                                        .Where(t => t.FeatureId == featureId);

            foreach (var ticket in tickets)
            {
                var ticketView = new TicketView(ticket);
                newCell.Items.Add(ticketView);

                ticketView.RequestEdit += (t) => editEntity(t);
                ticketView.RequestRemove += (t) => editEntity(t);
            }
        }

        #region Editing + Removing

        private void editEntity<T>(T entity) where T : Entity
        {
            var addEditView = new AddEditView(typeof(T), entity);
            addEditView.ShowDialog();

            Update();
        }

        private void removeEntity<T>(T entity) where T : Entity
        {
            if (MessageBox.Show("Are you sure you wish to delete this " + entity.GetType().Name + "?", "Scrumr", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            if (typeof(T) == typeof(Sprint))
                Sprints.Remove(entity as Sprint);
            else if (typeof(T) == typeof(Feature))
                Features.Remove(entity as Feature);
            else if (typeof(T) == typeof(Ticket))
                Tickets.Remove(entity as Ticket);

            Update();
        }

        #endregion

        #region event handlers

        void newCell_Drop(object sender, DragEventArgs e)
        {
            var cell = sender as ListBox;
            var targetSprintFeature = CellMap[cell];
            var ticket = e.Data.GetData(typeof(Ticket)) as Ticket;

            ticket.SprintId = targetSprintFeature.Sprint;
            ticket.FeatureId = targetSprintFeature.Feature;

            Update();
        }

        #endregion
    }
}
