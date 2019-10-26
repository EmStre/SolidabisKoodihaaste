using Newtonsoft.Json;
using System.Collections.Generic;

namespace BullshitChallenge.Models
{
    // Model of the data from the API.
    public class Bullshits
    {
        [JsonProperty("bullshits")]
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        [JsonProperty("message")]
        public string OneMessage { get; set; }
    }
}