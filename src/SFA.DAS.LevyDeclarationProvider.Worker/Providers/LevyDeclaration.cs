﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.EmployerLevy.Application.Commands.CreateEnglishFractionCalculationDate;
using SFA.DAS.EmployerLevy.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerLevy.Application.Commands.UpdateEnglishFractions;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public class LevyDeclaration : ILevyDeclaration
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        
        private static bool HmrcProcessingEnabled => CloudConfigurationManager.GetSetting("DeclarationsEnabled")
                                      .Equals("both", StringComparison.CurrentCultureIgnoreCase);

        private static bool DeclarationProcessingOnly => CloudConfigurationManager.GetSetting("DeclarationsEnabled")
            .Equals("declarations", StringComparison.CurrentCultureIgnoreCase);

        private static bool FractionProcessingOnly => CloudConfigurationManager.GetSetting("DeclarationsEnabled")
            .Equals("fractions", StringComparison.CurrentCultureIgnoreCase);

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILog logger)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();

                try
                {
                    if (HmrcProcessingEnabled || DeclarationProcessingOnly || FractionProcessingOnly)
                    {
                        await ProcessMessage(message);
                    }
                    else
                    {
                        //Ignore the message as we are not processing declarations
                        
                        if (message?.Content != null)
                        {
                            await message.CompleteAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, $"Levy declaration processing failed for paye scheme [{message?.Content?.PayeRef}]");
                    break; //Stop processing anymore messages as this failure needs to be investigated
                }
            }
        }

        private async Task ProcessMessage(Message<EmployerRefreshLevyQueueMessage> message)
        {
            if (message?.Content == null)
            {
                if (message != null)
                {
                    await message.CompleteAsync();
                }
                return;
            }
            var timer = Stopwatch.StartNew();

            var payeRef = message.Content.PayeRef;

            _logger.Trace($"Processing LevyDeclaration for paye scheme {payeRef}");
            
            var employerDataList = new List<EmployerLevyData>();

            var englishFractionUpdateResponse = await _mediator.SendAsync(new GetEnglishFractionUpdateRequiredRequest());

            await ProcessScheme(payeRef, englishFractionUpdateResponse, employerDataList);
            

            if (englishFractionUpdateResponse.UpdateRequired)
            {
                await _mediator.SendAsync(new CreateEnglishFractionCalculationDateCommand
                {
                    DateCalculated = englishFractionUpdateResponse.DateCalculated
                });
            }
            
            await _mediator.SendAsync(new RefreshEmployerLevyDataCommand
            {
                EmployerLevyData = employerDataList
            });
            
            await message.CompleteAsync();

            timer.Stop();
            _logger.Trace($"Finished processing LevyDeclaration for paye scheme {payeRef}. Completed in {timer.Elapsed:g} (hh:mm:ss:ms)");
        }

        private async Task ProcessScheme(string payeRef, GetEnglishFractionUpdateRequiredResponse englishFractionUpdateResponse, ICollection<EmployerLevyData> employerDataList)
        {
            if (HmrcProcessingEnabled || FractionProcessingOnly)
            {
                await _mediator.SendAsync(new UpdateEnglishFractionsCommand
                {
                    EmployerReference = payeRef,
                    EnglishFractionUpdateResponse = englishFractionUpdateResponse
                });
            }

            var levyDeclarationQueryResult = HmrcProcessingEnabled || DeclarationProcessingOnly ?
                await _mediator.SendAsync(new GetHMRCLevyDeclarationQuery {EmpRef = payeRef }) : null;

            var employerData = new EmployerLevyData();

            if (levyDeclarationQueryResult?.LevyDeclarations?.Declarations != null)
            {
                foreach (var declaration in levyDeclarationQueryResult.LevyDeclarations.Declarations)
                {
                    var dasDeclaration = new DasDeclaration
                    {
                        SubmissionDate = DateTime.Parse(declaration.SubmissionTime),
                        Id = declaration.Id,
                        PayrollMonth = declaration.PayrollPeriod?.Month,
                        PayrollYear = declaration.PayrollPeriod?.Year,
                        LevyAllowanceForFullYear = declaration.LevyAllowanceForFullYear,
                        LevyDueYtd = declaration.LevyDueYearToDate,
                        NoPaymentForPeriod = declaration.NoPaymentForPeriod,
                        DateCeased = declaration.DateCeased,
                        InactiveFrom = declaration.InactiveFrom,
                        InactiveTo = declaration.InactiveTo,
                        SubmissionId = declaration.SubmissionId
                    };

                    employerData.EmpRef = payeRef;
                    employerData.Declarations.Declarations.Add(dasDeclaration);
                }

                employerDataList.Add(employerData);
            }
        }
    }
}

