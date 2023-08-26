using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Models
{
	public class DraftReading
	{
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string Author { get; set; } = "";
        public string Port { get; set; } = "";
        public DateTime DatePublished { get; set; }
        public string? Condition { get; set; }
        [Range(4.0, 9.5, ErrorMessage="To be within 4.0-9.5 m range")]
        public decimal ObservedFwd { get; set; }
        [Range(4.0, 9.5, ErrorMessage = "To be within 4.0-9.5 m range")]
        public decimal ObservedAft { get; set; }
        [Range(4.0, 9.5, ErrorMessage = "To be within 4.0-9.5 m range")]
        public decimal LoadmasterFwd { get; set; }
        [Range(4.0, 9.5, ErrorMessage = "To be within 4.0-9.5 m range")]
        public decimal LoadmasterAft { get; set; }
        [Range(4.0, 9.5, ErrorMessage = "To be within 4.0-9.5 m range")]
        public decimal SensorFwd { get; set; }
        [Range(4.0, 9.5, ErrorMessage = "To be within 4.0-9.5 m range")]
        public decimal SensorAft { get; set; }
        public string? CorCargo { get; set; }
    }
}

