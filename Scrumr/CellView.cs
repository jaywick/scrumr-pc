using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scrumr
{
    class CellView : ListBox
    {
        public SprintFeature SprintFeature { get; private set; }

        public CellView(SprintFeature sprintFeature)
        {
            SprintFeature = sprintFeature;

            AllowDrop = true;
            Background = Brushes.Transparent;
        }
    }
}
