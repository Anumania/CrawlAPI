using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CrawlAPI
{
    static class Util
    {
        //assume you know what the return type will be and can cast it appropriately.
        public static object GetField(object _object, string fieldName)
        {
            return _object.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_object);
        }

        public static void SetField(object _object, string fieldName, object value)
        {
            _object.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(_object,value);
        }
    }
}
