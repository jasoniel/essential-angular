using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServerApp.Models.BindingTargets
{
    public class CheckoutState
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty("cardExpiry")]
        public string CardExpiry { get; set; }

        [JsonProperty("cardSecurityCode")]
        public string CardSecurityCode { get; set; }
    }
}
