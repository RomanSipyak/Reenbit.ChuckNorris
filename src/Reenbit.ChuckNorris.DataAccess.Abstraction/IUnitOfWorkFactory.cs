namespace Reenbit.ChuckNorris.DataAccess.Abstraction
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}
