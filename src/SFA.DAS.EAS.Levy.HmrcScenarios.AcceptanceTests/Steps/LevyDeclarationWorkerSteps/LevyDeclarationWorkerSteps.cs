﻿using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerLevy.TestCommon.ScenarioCommonSteps;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerLevy.HmrcScenarios.AcceptanceTests.Steps.LevyDeclarationWorkerSteps
{
    [Binding]
    public class LevyDeclarationWorkerSteps
    {
        private static LevyWorkerSteps _levyWorkerSteps;

        [BeforeFeature]
        public static void Arrange()
        {
            _levyWorkerSteps = new LevyWorkerSteps();
        }

        [AfterFeature]
        public static void TearDown()
        {
            _levyWorkerSteps.Dispose();
        }

        [When(@"I get the following declarations from HMRC")]
        public void WhenIGetTheFollowingDeclarationsFromHMRC(Table table)
        {
            var groups = table.Rows.GroupBy(row => row["PAYEScheme"]).ToList();

            var hmrcResponses = new List<GetHMRCLevyDeclarationResponse>();
            
            foreach (var levyGroup in groups)
            {
                var response = new GetHMRCLevyDeclarationResponse
                {
                    Empref = levyGroup.Key,
                    LevyDeclarations = new LevyDeclarations
                    {
                        EmpRef = levyGroup.Key,
                        Declarations = levyGroup.Select(row => new Declaration
                            {
                                Id = row["Id"],
                                SubmissionId = string.IsNullOrEmpty(row["SubmissionId"]) ? 0 : Convert.ToInt64(row["SubmissionId"]),
                                DateCeased = string.IsNullOrEmpty(row["DateCeased"]) ? default(DateTime) : Convert.ToDateTime(row["DateCeased"]),
                                LevyDueYearToDate = string.IsNullOrEmpty(row["LevyDueYTD"]) ? 0 : Convert.ToDecimal(row["LevyDueYTD"]),
                                NoPaymentForPeriod = !string.IsNullOrEmpty(row["NoPaymentForPeriod"]) && Convert.ToBoolean(row["NoPaymentForPeriod"]),
                                SubmissionTime = row["SubmissionTime"],
                                LevyAllowanceForFullYear = string.IsNullOrEmpty(row["LevyAllowanceForFullYear"]) ? 0 : Convert.ToDecimal(row["LevyAllowanceForFullYear"]),
                                PayrollPeriod = new PayrollPeriod
                                {
                                    Month = Convert.ToInt16(row["PayrollMonth"]),
                                    Year = row["PayrollPeriod"]
                                }
                        }).ToList()
                    }
                };
                
                hmrcResponses.Add(response);

                _levyWorkerSteps.RunWorker(hmrcResponses);
            }
        }
    }
}
