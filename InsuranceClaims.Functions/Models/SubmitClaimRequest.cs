namespace InsuranceClaims.Functions.Models;

public class SubmitClaimRequest
{
    public string PolicyNumber { get; set; } = string.Empty;

    public DateTime LossDate { get; set; }

    public string LossType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal EstimatedDamage { get; set; }
}