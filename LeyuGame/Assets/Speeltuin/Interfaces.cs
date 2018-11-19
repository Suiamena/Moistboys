public interface IBehaviour
{
	void Execute ();
}

public interface IData<T>
{
	T YieldData ();
}