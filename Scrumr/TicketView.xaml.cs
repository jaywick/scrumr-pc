using System;
using System.Collections.Generic;
using System.Text;
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
	/// Interaction logic for TicketView.xaml
	/// </summary>
	public partial class TicketView : UserControl
	{
		public TicketView()
		{
			this.InitializeComponent();
		}

        public string ID { get; set; }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                labelName.Content = name;
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as TicketView, new DataObject("myFormat", sender), DragDropEffects.Move);
            }
        }
	}
}