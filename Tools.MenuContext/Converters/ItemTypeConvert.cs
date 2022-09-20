using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.MenuContext.Converters
{
    public class ItemTypeConvert : IValueConverter
    {
        //static Type Val = typeof(ival);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type t = value.GetType();
            PropertyInfo propertyInfo = t.GetProperty("ItemValue");
            string typeName = propertyInfo.PropertyType.Name.ToLower();
            return typeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
