using System.Threading.Tasks;
using System;
using System.Threading;

namespace Cimon.Jenkins;

public interface IJenkinsClient : IDisposable
{
	Task<TResult?> Get<TResult>(Query<TResult> query, CancellationToken token = default);
	Task Post(Command command, CancellationToken token = default);
}
