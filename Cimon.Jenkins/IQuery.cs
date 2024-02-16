using System.Net.Http;

namespace Cimon.Jenkins;

public interface IQuery<out TResult>
{
	string GetPath();
	HttpMethod Method => HttpMethod.Get;
	bool AddApiJsonSuffix => true;
}