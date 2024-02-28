using System.Threading.Tasks;
using System;
using System.Threading;

namespace Cimon.Jenkins;

public interface IJenkinsClient : IDisposable
{
    Task<TResult?> Get<TResult>(IQuery<TResult> query,CancellationToken ctx = default);
    /*
    Task BuildProject(string job, CancellationToken ctx = default(CancellationToken));
    Task BuildProjectWithParameters(string job, IDictionary<string, string> parameters, CancellationToken ctx = default(CancellationToken));
    Task CopyJob(string fromJobName, string newJobName, CancellationToken ctx = default(CancellationToken));
    Task CopyJob(string fromJobName, string newJobName, string path, CancellationToken ctx = default(CancellationToken));
    Task UploadJobConfig(string job, string xml, CancellationToken ctx = default(CancellationToken));
    Task EnableJob(string job, CancellationToken ctx = default(CancellationToken));
    Task DisableJob(string job, CancellationToken ctx = default(CancellationToken));
    Task DeleteJob(string job, CancellationToken ctx = default(CancellationToken));
    Task CreateJob(string job, string xml, CancellationToken ctx = default(CancellationToken));
    Task CreateJob(string job, string xml, string path, CancellationToken ctx = default(CancellationToken));
    Task CreateFolder(string folder, CancellationToken ctx = default(CancellationToken));
    Task CreateFolder(string folder, string path, CancellationToken ctx = default(CancellationToken));
    Task DeleteFolder(string folder, CancellationToken ctx = default(CancellationToken));
    Task QuietDown(string reason = "", CancellationToken ctx = default(CancellationToken));
    Task CancelQuietDown(CancellationToken ctx = default(CancellationToken));
    Task Restart(CancellationToken ctx = default(CancellationToken));
    Task SafeRestart(CancellationToken ctx = default(CancellationToken));*/
}
