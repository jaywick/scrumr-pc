using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrumr
{
    public class SprintFeature
    {
        public int Sprint { get; set; }
        public int Feature { get; set; }

        public SprintFeature(int sprint, int feature)
        {
            this.Sprint = sprint;
            this.Feature = feature;
        }
    }
}
