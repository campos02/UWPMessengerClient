﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.UI.Xaml.Data;

namespace UWPMessengerClient
{
    class UrlDecodingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string urlEncodedString = (string)value;
            string urlDecodedString = HttpUtility.UrlDecode(urlEncodedString);
            return urlDecodedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
