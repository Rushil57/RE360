﻿using RE360.API.DBModels;
using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class ExecutionViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public string? AdditionalDisclosures { get; set; }
        public string? SignedOnBehalfOfTheAgent { get; set; }

        public DateTime? SignedOnBehalfOfTheAgentDate { get; set; }
        public string? SignedOnBehalfOfTheAgentTime { get; set; }
        public string? AgentToSignHere { get; set; }
        public string? AgentToSignHereDate { get; set; }
        public List<SignaturesOfClientViewModel> SignaturesOfClient { get; set; }
    }
}
