using System.Threading.Tasks;
using System;
using System.Threading;

namespace Cimon.Jenkins;

public interface IJenkinsClient : IDisposable
{
	Task<TResult?> Query<TResult>(Query<TResult> query, CancellationToken token = default);
	Task Exec(Command command, CancellationToken token = default);
}
