namespace Ae.Poc.Identity.Settings;

 public sealed class HealthOptions
 {
     public const string Health = "Health";

     public bool Enabled { get; set; } = true;
     public int? Port { get; set; }
     public string LivePath { get; set; } = "/health/live";
     public string ReadyPath { get; set; } = "/health/ready";
 }