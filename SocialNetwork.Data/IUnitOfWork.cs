namespace SocialNetwork.Data
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges(bool ensureAutohistory = false);

        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
    }
}
