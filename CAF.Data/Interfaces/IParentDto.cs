namespace CompositeApplicationFramework.Interfaces
{
    public interface IParentDto<out TKey> : IId<TKey>, IDto
    {
    }
}