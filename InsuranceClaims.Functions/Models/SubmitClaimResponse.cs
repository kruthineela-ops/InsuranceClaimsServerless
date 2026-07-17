namespace InsuranceClaims.Functions.Models;

public class SubmitClaimResponse
{
    public string ClaimNumber { get; set; } = string.Empty;

    public string PolicyNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime SubmittedAtUtc { get; set; }

    public string Message { get; set; } = string.Empty;
}