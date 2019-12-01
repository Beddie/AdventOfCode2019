using Logic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logic.Service
{
    public class RenderDayService
    {
        public static AdventBase GetDay(int day, EnumParts part = EnumParts.None)
        {
            try
            {
                Type elementType = Type.GetType($"Logic.Days.Day{day}");
                var dayObject = (AdventBase)Activator.CreateInstance(elementType, day);
                if (part != EnumParts.None) { dayObject.PartsToRender = new List<EnumParts>() { part }; }; 
                if (dayObject.Active) return dayObject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        public static List<AdventBase> GetOverview()
        {
            var overviewList = new List<AdventBase>();
            for (int i = 1; i < 26; i++)
            {
                var day = GetDay(i);
                if (day != null) overviewList.Add(day);
            }
            return overviewList;
        }

    }
}
