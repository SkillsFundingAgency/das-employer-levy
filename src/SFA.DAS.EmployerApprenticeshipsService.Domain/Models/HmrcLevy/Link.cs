using Newtonsoft.Json;

namespace SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy
{
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}