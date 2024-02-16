namespace Cimon.Jenkins;

public abstract record Query<T> : IQuery<T>
{
	public abstract string GetPath();

    
}