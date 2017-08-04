﻿using System;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerLevy.Application.UnitTests
{
    public class FakeTimeProvider : DateTimeProvider
    {
        public FakeTimeProvider(DateTime input)
        {
            UtcNow = input;
        }

        public override DateTime UtcNow { get; }
    }
}