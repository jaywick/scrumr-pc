using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr.Client
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(PropertyItem item)
            : base(String.Format("Please enter a valid value for {0}", item.Name))
        {
        }
    }
}
