using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Models
{
    public class RmProjectForecast
    {
        public RmProjectForecast(List<string> project)
        {
            Group = project.ElementAt(1);
            Resource = project.ElementAt(2).Replace("Total","").Trim();
            Project = project.ElementAt(3);
            Sales_Status = project.ElementAt(4);
            Acct_Mgr = project.ElementAt(5);
            Lead = project.ElementAt(6);

            Forecast.Add(project.ElementAt(7));
            Forecast.Add(project.ElementAt(8));
            Forecast.Add(project.ElementAt(9));
            Forecast.Add(project.ElementAt(10));
            Forecast.Add(project.ElementAt(11));
        }

        public string Group { get; set; }

        public string Resource { get; set; }

        public string Project { get; set; }

        public string Sales_Status { get; set; }

        public string Acct_Mgr { get; set; }

        public string Lead { get; set; }

        public List<string> Forecast = new List<string>();

        public string GetForecastMessageForWeek(int week)
        {
            try
            {
                if (string.IsNullOrEmpty(Project))
                {
                    return $"{Resource} has {Forecast.ElementAt(week)} total hours projected.";
                }

                return $"For project {Project}, {Resource} has {Forecast.ElementAt(week)} hours projected.";
            }
            catch
            {
                return $"Could not find forecast for week {week}. I can project up to {Forecast.Count} weeks out, including last week.";
            }
        }
    }
}