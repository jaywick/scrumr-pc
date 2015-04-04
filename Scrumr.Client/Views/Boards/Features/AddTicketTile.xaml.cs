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
using System.Diagnostics;

namespace Scrumr.Client
{
    partial class AddTicketTile : UserControl
    {
        public event Action<Ticket> Added;

        private Feature Feature { get; set; }
        private Sprint Sprint { get; set; }

        private Process _oskProcess;

        public AddTicketTile(Feature feature, Sprint sprint)
        {
            InitializeComponent();
            this.Feature = feature;
            this.Sprint = sprint ?? feature.Project.Backlog;

            this.PreviewMouseDown += AddButtonTileView_PreviewMouseDown;
            this.PreviewLostKeyboardFocus += (s,e) => Reset();
            TicketSummary.PreviewKeyDown += TicketSummary_PreviewKeyDown;
        }

        void AddButtonTileView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartQuickEdit();
            e.Handled = true;
        }

        public void StartQuickEdit()
        {
            _oskProcess = Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");

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
                Added.Invoke(new Ticket(Feature.Context)
                {
                    Name = TicketSummary.Text,
                    Description = "",
                    State = TicketState.Open,
                    Type = TicketType.Task,
                    SprintId = Sprint.ID,
                    FeatureId = Feature.ID,
                });

                HideKeyboard();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Reset();
                HideKeyboard();
                e.Handled = true;
            }
        }

        private void HideKeyboard()
        {
            Process.GetProcessesByName("tabtip")
                .ToList()
                .ForEach(x => x.Kill());
        }
    }
}
