﻿using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class CommitmentListViewModel
    {
        public string AccountHashId { get; set; }
        public List<CommitmentListItemViewModel> Commitments { get; set; }
        public int NumberOfTasks { get; set; }
    }
}