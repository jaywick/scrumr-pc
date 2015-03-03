using Scrumr.Database;
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
using Scrumr.Client;

namespace Scrumr.Client
{
    partial class AddButtonTileView : UserControl
    {
        public event Action<Ticket> Added;

        private Feature Feature { get; set; }
        private Sprint Sprint { get; set; }

        public AddButtonTileView(Feature feature, Sprint sprint)
        {
            InitializeComponent();
            this.Feature = feature;
            this.Sprint = sprint;

            this.PreviewMouseDown += AddButtonTileView_PreviewMouseDown;
            this.PreviewLostKeyboardFocus += (s,e) => Reset();
            TicketSummary.PreviewKeyDown += TicketSummary_PreviewKeyDown;
        }

        void AddButtonTileView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartQuickEdit();
            e.Handled = true;
        }

        private void StartQuickEdit()
        {
            TicketSummary.Visibility = Visibility.Visible;
            AddIconLabel.Visibility = Visibility.Collapsed;

            TicketSummary.Focus();
        }

        public void Reset()
        {
            TicketSummary.Visibility = Visibility.Collapsed;
            AddIconLabel.Visibility = Visibility.Visible;

            TicketSummary.Text = "";
        }

        private void TicketSummary_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Added.Invoke(new Ticket
                {
                    Name = TicketSummary.Text,
                    Description = "",
                    State = TicketState.Open,
                    Type = TicketType.Task,
                    SprintId = Sprint.ID,
                    FeatureId = Feature.ID,
                });

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Reset();
                e.Handled = true;
            }
        }
    }
}
