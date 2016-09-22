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
            var client = new RestClient("https://microsoft-apiapp7408f19c5a984ba2ac3d7c693d65f1f9.azurewebsites.net");
            var request = new RestRequest("api/values", Method.GET);
            var response = client.Execute<RmifyResponse>(request);

            return response.Data;
        }

        public static List<RmProjectForecast> GetForecastForSherpa(string sherpa)
        {
            var rmData = GetRmData();
            rmData.Results = rmData.Results.Where(x => x.Key.Contains(sherpa)).ToDictionary(x => x.Key, x => x.Value);

            return rmData.Results.Values.First().Select(project => new RmProjectForecast(project)).ToList();
        }
    }
}