# Cimon.Jenkins [![NuGet](https://img.shields.io/nuget/v/Cimon.Jenkins.svg)](https://www.nuget.org/packages/Cimon.Jenkins/)
This is a fork of Narochno.Jenkins a simple Jenkins client, providing a C# wrapper around the default Jenkins API.
This implementation uses IHttpClientFactory to create HTTP client.

## Implemented API
- Query info:
    - user
    - server
    - view
    - job
    - build info, console output
    - test report
- Post commands:
    - start build (with parameters)
    - create, copy, edit, remove jobs and folders
    - disable/enable jobs
    - restart, set quiet mode on/off

## Example Usage
See examples in [JenkinsClientIntegrationTests](/Cimon.Jenkins.Tests/JenkinsClientIntegrationTests.cs)
