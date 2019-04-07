#region Usings

using CompositeApplicationFramework.Factory;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    public static class DtoExtensions
    {
        public static IEntity ToEntity(this IDto dto)
        {
            var factory = EntityFactory.GetFactory(dto.GetType());

            return factory.Create(dto) as IEntity;
        }

        public static TEntity ToEntity<TEntity>(this IDto dto)
        {
            var factory = EntityFactory.GetFactory<IFactory<TEntity>>(dto.GetType());

            return factory.Create(dto);
        }
    }
}