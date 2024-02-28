using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public interface IQuery<TResult>
{
	string GetPath();
	HttpMethod Method => HttpMethod.Get;
	bool AddApiJsonSuffix { get; }
	Task<TResult?> GetResult(HttpResponseMessage response, CancellationToken ctx);
}
