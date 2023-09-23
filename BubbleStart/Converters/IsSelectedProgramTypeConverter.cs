﻿using BubbleStart.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BubbleStart.Converters
{
    public class IsSelectedProgramTypeConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Customer customer)
            {
                return customer.DefaultProgramModes?.Split(',').Contains((parameter ?? "-").ToString());
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string date && parameter is string par)
            {
                if (!DateTime.TryParseExact(date, par, CultureInfo.CurrentUICulture, DateTimeStyles.None, out DateTime datet))
                {
                    return null;
                }
                return datet;
            }
            return null;
        }

        #endregion Methods
    }
}