using System.Diagnostics;

namespace Orders.Api.OpenTelemetry;

internal static class DiagnosticConfig
{
    // Custom ActivitySource for Orders-Api

    internal static readonly ActivitySource ActivitySource = new("Orders-Api");
}
