using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using RezaBot.Models;

namespace RezaBot.Services
{
    public static class RmService
    {
        private static RmifyResponse GetRmData()
        {
            var client = new RestClient("http://rm-api.azurewebsites.net");
            var request = new RestRequest("api/rm", Method.GET);
            var response = client.Execute<RmifyResponse>(request);

            return response.Data;
        }

        public static List<RmProjectForecast> GetForecastForSherpa(string sherpa)
        {
            if (string.IsNullOrEmpty(sherpa))
            {
                return new List<RmProjectForecast>();
            }

            var rmData = GetRmData();
            rmData.Sherpas = rmData.Sherpas.Where(x => x.Key.Contains(sherpa)).ToDictionary(x => x.Key, x => x.Value);

            if (!rmData.Sherpas.Any())
            {
                return new List<RmProjectForecast>();
            }

            return rmData.Sherpas.First().Value.Forecasts;
        }
    }
}