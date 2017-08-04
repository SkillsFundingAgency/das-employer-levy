﻿using Newtonsoft.Json;

namespace SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy
{
    public class EmpRefLevyInformation
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }
        [JsonProperty("employer")]
        public Employer Employer { get; set; }
    }
}