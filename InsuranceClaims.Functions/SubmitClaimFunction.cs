using System.Net;
using System.Text.Json;
using InsuranceClaims.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace InsuranceClaims.Functions;

public class SubmitClaimFunction
{
    private readonly ILogger<SubmitClaimFunction> _logger;

    public SubmitClaimFunction(
        ILogger<SubmitClaimFunction> logger)
    {
        _logger = logger;
    }

    [Function("SubmitClaim")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = "claims")]
        HttpRequestData request)
    {
        _logger.LogInformation(
            "Claim submission request received.");

        SubmitClaimRequest? claimRequest;

        try
        {
            claimRequest =
                await JsonSerializer.DeserializeAsync<SubmitClaimRequest>(
                    request.Body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(
                exception,
                "Invalid JSON was submitted.");

            HttpResponseData badJsonResponse =
                request.CreateResponse(HttpStatusCode.BadRequest);

            await badJsonResponse.WriteAsJsonAsync(new
            {
                message = "The request contains invalid JSON."
            });

            return badJsonResponse;
        }

        if (claimRequest is null)
        {
            HttpResponseData emptyRequestResponse =
                request.CreateResponse(HttpStatusCode.BadRequest);

            await emptyRequestResponse.WriteAsJsonAsync(new
            {
                message = "Claim information is required."
            });

            return emptyRequestResponse;
        }

        if (string.IsNullOrWhiteSpace(claimRequest.PolicyNumber))
        {
            HttpResponseData validationResponse =
                request.CreateResponse(HttpStatusCode.BadRequest);

            await validationResponse.WriteAsJsonAsync(new
            {
                message = "Policy number is required."
            });

            return validationResponse;
        }

        string claimNumber =
            $"CLM-{DateTime.UtcNow:yyyyMMdd}-" +
            Guid.NewGuid()
                .ToString("N")[..8]
                .ToUpperInvariant();

        SubmitClaimResponse responseBody = new()
        {
            ClaimNumber = claimNumber,
            PolicyNumber = claimRequest.PolicyNumber,
            Status = "Submitted",
            SubmittedAtUtc = DateTime.UtcNow,
            Message = "The claim was submitted successfully."
        };

        _logger.LogInformation(
            "Claim {ClaimNumber} submitted for policy {PolicyNumber}.",
            claimNumber,
            claimRequest.PolicyNumber);

        HttpResponseData response =
            request.CreateResponse(HttpStatusCode.Created);

        await response.WriteAsJsonAsync(responseBody);

        return response;
    }
}