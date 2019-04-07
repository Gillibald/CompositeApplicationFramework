namespace CompositeApplicationFramework.Interfaces
{
    public interface IEntity
    {
    }

    public interface IEntity<out TId> : IEntity, IId<TId>
    {
    }
}