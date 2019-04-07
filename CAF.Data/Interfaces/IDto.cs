namespace CompositeApplicationFramework.Interfaces
{
    public interface IDto
    {
    }

    public interface IDto<out TId> : IDto, IId<TId>
    {
    }
}