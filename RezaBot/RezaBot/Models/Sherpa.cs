using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Models
{
    public class Sherpa
    {
        public Sherpa()
        {
            Forecasts = new List<RmProjectForecast>();
        }

        public string Name { get; set; }

        public List<RmProjectForecast> Forecasts { get; set; }

        public string Competency
        {
            // Return the competency listed in the resource's "Total" row
            get
            {
                if (Forecasts == null || Forecasts.Count == 0)
                {
                    return string.Empty;
                }
                return Forecasts[Forecasts.Count() - 1].Group;
            }
        }

        public string DocumentID { get; set; }

        public string InternalID { get; set; }

        public int GetTotalHoursForWeek(RmProjectForecast.WeekValues week) => GetTotalHoursForWeek((int)week);

        public int GetTotalHoursForWeek(int week) => Forecasts.Take(Forecasts.Count - 1).Sum(x => x.Forecast[week]);
    }
}