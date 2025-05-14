namespace AddPulseShowManagement.Repo
{
    public interface IFactory<T> where T : IRepository
    {
        TBase CreateInstance<TBase>(IServiceProvider provider) where TBase : class;
    }
}
