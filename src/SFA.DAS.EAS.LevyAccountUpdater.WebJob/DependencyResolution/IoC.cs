// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob.DependencyResolution {
    public static class IoC
    {
        private const string ServiceName = "SFA.DAS.EmployerLevy";

        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerLevyConfiguration>(ServiceName));
                c.Policies.Add(new MessagePolicy<EmployerLevyConfiguration>(ServiceName));
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}