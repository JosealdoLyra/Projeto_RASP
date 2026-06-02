using System;

namespace Rasp.Api.Models
{
    public class OnePageRaspEntity
    {
        public int IdOnepageRasp { get; set; }

        public int IdRasp { get; set; }

        public string? IssueDescription { get; set; }

        public string? PreliminaryRootCause { get; set; }

        public string? ContainmentAction { get; set; }

        public string? RootCauseAnalysis { get; set; }

        public string? BreakingPoint { get; set; }

        public string? ImagemPath { get; set; }

        public DateTime? DataCriacao { get; set; }

        public int? IdUsuarioCriacao { get; set; }
    }
}
