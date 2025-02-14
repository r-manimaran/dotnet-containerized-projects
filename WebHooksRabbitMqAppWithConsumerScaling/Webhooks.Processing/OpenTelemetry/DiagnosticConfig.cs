using System.Diagnostics;

namespace Webhooks.Processing.OpenTelemetry;

internal static class DiagnosticConfig
{
    // Custom ActivitySource for Orders-Api

    internal static readonly ActivitySource ActivitySource = new("webhooks-processing");
}
