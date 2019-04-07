#region Usings

using System.Windows.Controls;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     Grid extension
    /// </summary>
    public static class GridExtension
    {
        /// <summary>
        ///     Get child control from grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static T GwnGetChildControl<T>(this Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is T)
                {
                    return (T) child;
                }
            }
            return default(T);
        }
    }
}