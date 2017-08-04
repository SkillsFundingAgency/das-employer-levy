using Newtonsoft.Json;

namespace SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy
{
    public class Employer
    {
        [JsonProperty("name")]
        public Name Name { get; set; }
    }
}