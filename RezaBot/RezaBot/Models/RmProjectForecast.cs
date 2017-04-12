using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Models
{
    public class RmProjectForecast
    {
        // Helper enum for popular weeks
        public enum WeekValues
        {
            LastWeek = 0,
            ThisWeek = 1,
            NextWeek = 2
        }

        public RmProjectForecast()
        {
            Forecast = new List<int>();
        }

        public RmProjectForecast(List<string> project)
        {
            Indicator = project.ElementAt(0);
            Group = project.ElementAt(1);
            Resource = project.ElementAt(2).Replace("Total", "").Trim();
            Project = project.ElementAt(3);
            Sales_Status = project.ElementAt(4);
            Acct_Mgr = project.ElementAt(5);
            Lead = project.ElementAt(6);

            Forecast = new List<int>();

            // RM goes out 16 weeks
            for (int x = 0; x < 16; x++)
            {
                if (x + 7 >= project.Count)
                {
                    break;
                }

                int num;

                if (!int.TryParse(project.ElementAt(x + 7), out num))
                {
                    num = 0;
                }

                Forecast.Add(num);
            }
        }

        public string Indicator { get; set; }

        public string Group { get; set; }

        public string Resource { get; set; }

        public string Project { get; set; }

        public string Sales_Status { get; set; }

        public string Acct_Mgr { get; set; }

        public string Lead { get; set; }

        public List<int> Forecast { get; set; }

        public string GetForecastMessageForWeek(WeekValues week)
        {
            return GetForecastMessageForWeek((int)week);
        }

        public string GetForecastMessageForWeek(int week)
        {
            try
            {
                var hours = Forecast.ElementAt(week);

                if (hours <= 0)
                {
                    return "";
                }
                if (string.IsNullOrEmpty(Project))
                {
                    return $"{Resource} has {hours} total hours projected.";
                }

                return $"For project {Project}, {Resource} has {Forecast.ElementAt(week)} hours projected.";
            }
            catch
            {
                return $"Could not find forecast for week {week}. I can project up to {Forecast.Count()} weeks out, including last week.";
            }
        }
    }
}