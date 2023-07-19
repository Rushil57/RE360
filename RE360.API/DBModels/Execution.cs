using RE360.API.Migrations;

namespace RE360.API.DBModels
{
    public class Execution
    {
        public int ID { get; set; }
        public int PID { get; set; }
        public string? AdditionalDisclosures { get; set; }
        public string? SignedOnBehalfOfTheAgent { get; set; }

        public DateTime? SignedOnBehalfOfTheAgentDate { get; set; }
        public string? SignedOnBehalfOfTheAgentTime { get; set; }
        public string? AgentToSignHere { get; set; }
        public string? AgentToSignHereDate { get; set; }
        public List<SignaturesOfClient> SignaturesOfClient { get; set; }

    }
}
