#region Usings

using CompositeApplicationFramework.Factory;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    public static class EntityExtensions
    {
        public static IDto ToDto(this IEntity entity)
        {
            var factory = DtoFactory.GetFactory(entity.GetType());

            return factory.Create(entity) as IDto;
        }

        public static TDto ToDto<TDto>(this IEntity entity)
        {
            var factory = DtoFactory.GetFactory<IFactory<TDto>>(entity.GetType());

            return factory.Create(entity);
        }
    }
}