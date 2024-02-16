﻿using System;

namespace Cimon.Jenkins;

public class JenkinsConfig
{
    /// <summary>
    /// The Jenkins base URL
    /// </summary>
    public required Uri JenkinsUrl { get; set; }

    /// <summary>
    /// The username to authenticate with
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The API key for the supplied user
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// The number of times requests will be retried 
    /// </summary>
    public int RetryAttempts { get; set; } = 2;

    /// <summary>
    /// The number of retries will be an exponent of this number
    /// </summary>
    public int RetryBackoffExponent { get; set; } = 2;
}
