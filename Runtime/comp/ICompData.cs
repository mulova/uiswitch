namespace mulova.ui
{
    public interface ICompData<T>
    {
        void ApplyTo(T o);
        void Collect(T o);
    }
}

