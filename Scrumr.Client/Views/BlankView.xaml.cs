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

namespace Scrumr.Client
{
    public partial class BlankView : UserControl
    {
        public event Action RequestNew;
        public event Action RequestOpen;

        public BlankView()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return MessageText.Content.ToString(); }
            set { MessageText.Content = value; }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (RequestOpen != null)
                RequestOpen.Invoke();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (RequestNew != null)
                RequestNew.Invoke();
        }
    }
}
