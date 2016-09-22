using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RezaBot.Services;

namespace BotTests.RM
{
    [TestClass]
    public class HivecastTests
    {
        [TestMethod]
        public void CanGetHivecast()
        {
            var forecast = RmService.GetForecastForSherpa("Cody Swendrowski");

            Assert.IsNotNull(forecast);

            foreach (var message in forecast.Where(x => !string.IsNullOrEmpty(x.Forecast.ElementAt(2))))
            {
                Assert.IsNotNull(message);
                var forecastMessage = message.GetForecastMessageForWeek(2);
                Assert.IsFalse(string.IsNullOrEmpty(forecastMessage));
                Trace.WriteLine(forecastMessage);
            }
        }
    }
}
