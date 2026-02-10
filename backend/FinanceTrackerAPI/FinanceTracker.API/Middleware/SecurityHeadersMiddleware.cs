namespace FinanceTrackerAPI.FinanceTracker.API.Middleware
{
    /// <summary>
    /// Adds security headers to all HTTP responses.
    ///
    /// OWASP Recommended Headers:
    /// https://owasp.org/www-project-secure-headers/
    ///
    /// These headers instruct browsers how to handle content securely.
    /// They are a defense-in-depth measure - they don't replace proper coding practices.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers BEFORE the response is sent
            var headers = context.Response.Headers;

            // ================================================================
            // X-Content-Type-Options: nosniff
            // ================================================================
            // Prevents MIME-type sniffing attacks.
            // Without this, browser might execute a file as JavaScript
            // even if Content-Type says it's text/plain.
            //
            // Attack example: Attacker uploads malicious.txt containing JS,
            // browser "sniffs" it as JavaScript and executes it.
            headers.Append("X-Content-Type-Options", "nosniff");

            // ================================================================
            // X-Frame-Options: DENY
            // ================================================================
            // Prevents clickjacking attacks.
            // Your site cannot be embedded in iframes.
            //
            // Attack example: Attacker creates iframe of your banking page,
            // overlays fake UI, tricks user into clicking "Transfer $10000".
            //
            // Options:
            // - DENY: Never allow framing
            // - SAMEORIGIN: Only allow framing by same origin
            // - ALLOW-FROM uri: Only specific origin (deprecated)
            headers.Append("X-Frame-Options", "DENY");

            // ================================================================
            // X-XSS-Protection: 0
            // ================================================================
            // Modern recommendation is to DISABLE this header (set to 0).
            // The browser's XSS auditor is deprecated and can actually
            // introduce vulnerabilities. CSP is the modern replacement.
            //
            // Previously was "1; mode=block" but that caused issues.
            headers.Append("X-XSS-Protection", "0");

            // ================================================================
            // Referrer-Policy: strict-origin-when-cross-origin
            // ================================================================
            // Controls what's sent in the Referer header.
            //
            // Privacy concern: Full URL might contain sensitive data like
            // /reset-password?token=abc123
            //
            // Options:
            // - no-referrer: Never send
            // - strict-origin-when-cross-origin: Full URL same-origin, origin only cross-origin
            // - same-origin: Only send for same-origin requests
            headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // ================================================================
            // Permissions-Policy (formerly Feature-Policy)
            // ================================================================
            // Disables browser features we don't need.
            // Reduces attack surface if attacker injects code.
            //
            // Format: feature=(allowlist)
            // () = disabled, (self) = only this origin
            headers.Append("Permissions-Policy",
                "accelerometer=(), " +
                "camera=(), " +
                "geolocation=(), " +
                "gyroscope=(), " +
                "magnetometer=(), " +
                "microphone=(), " +
                "payment=(), " +
                "usb=()");

            // ================================================================
            // Content-Security-Policy
            // ================================================================
            // The most powerful security header. Whitelist of allowed sources.
            //
            // For an API, we have a simple policy since we don't serve HTML:
            // - default-src 'none': Block everything by default
            // - frame-ancestors 'none': Like X-Frame-Options but more powerful
            //
            // For a frontend app, you'd allow specific script/style sources.
            headers.Append("Content-Security-Policy",
                "default-src 'none'; " +
                "frame-ancestors 'none'; " +
                "base-uri 'none'; " +
                "form-action 'none'");

            // ================================================================
            // Cache-Control for sensitive responses
            // ================================================================
            // Prevent caching of sensitive API responses.
            // This is especially important for authenticated endpoints.
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
                headers.Append("Pragma", "no-cache");
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method for cleaner middleware registration.
    /// Usage: app.UseSecurityHeaders();
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
