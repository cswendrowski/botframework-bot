using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Models
{
    public class RmifyResponse
    {
        public List<string> Legend { get; set; }

        public List<string> Competency_List { get; set; }

        public Dictionary<string, Sherpa> Sherpas { get; set; }
    }
}