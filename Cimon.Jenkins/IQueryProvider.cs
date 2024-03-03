using Cimon.Jenkins.Entities.Views;

namespace Cimon.Jenkins;

public interface IQueryProvider
{
	Query<ViewInfo> ToQuery();
}
