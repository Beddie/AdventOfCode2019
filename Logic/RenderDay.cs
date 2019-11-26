using Logic.Days;
using System;
using System.Collections.Generic;

namespace Logic
{
    public class RenderDay
    {
        public static AdventBase GetDay(int day)
        {
            try
            {
                Type elementType = Type.GetType($"Logic.Days.Day{day}");
                var dayObject = (AdventBase)Activator.CreateInstance(elementType);
                if (dayObject.Active) return dayObject;
            }
            catch (Exception)
            {
                //Do nothing
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
