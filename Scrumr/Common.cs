﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Scrumr
{
    class Common
    {
        public static MenuItem CreateMenuItem(string text, System.Action action)
        {
            var newItem = new MenuItem();
            newItem.Header = text;
            newItem.Click += (s, e) => action.Invoke();

            return newItem;
        }

        public class InvalidInputException : Exception
        {
            public InvalidInputException(PropertyItem item)
                : base(String.Format("Please enter a valid value for {0}", item.Name))
            {
            }
        }
    }
}