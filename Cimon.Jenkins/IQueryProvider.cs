using Cimon.Jenkins.Entities.Views;

namespace Cimon.Jenkins;

public interface IQueryProvider
{
	IQuery<ViewInfo> ToQuery();
}
