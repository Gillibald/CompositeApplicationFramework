#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CompositeApplicationFramework.Behaviors.DragDropBehavior.DragDropAdorner;
using CompositeApplicationFramework.Helper;

#endregion

namespace CompositeApplicationFramework.Behaviors.DragDropBehavior
{
    #region Dependencies

    

    #endregion

    public class DefaultDropHandler : IDropTarget
    {
        public virtual void DragOver(IDropInfo pDropInfo)
        {
            if (!CanAcceptData(pDropInfo)) return;
            pDropInfo.Effects = DragDropEffects.Copy;
            pDropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        public virtual void Drop(IDropInfo pDropInfo)
        {
            var lInsertIndex = pDropInfo.InsertIndex;
            var lDestinationList = GetList(pDropInfo.TargetCollection);
            var lData = ExtractData(pDropInfo.Data);

            var lItems = lData as object[] ?? lData.Cast<object>().ToArray();
            if (Equals(pDropInfo.DragInfo.VisualSource, pDropInfo.VisualTarget))
            {
                var sourceList = GetList(pDropInfo.DragInfo.SourceCollection);

                foreach (var lIndex in lItems
                    .Select(lItem => sourceList.IndexOf(lItem))
                    .Where(lIndex => lIndex != -1))
                {
                    sourceList.RemoveAt(lIndex);

                    if (sourceList == lDestinationList && lIndex < lInsertIndex)
                    {
                        --lInsertIndex;
                    }
                }
            }

            foreach (var lItem in lItems)
            {
                lDestinationList.Insert(lInsertIndex++, lItem);
            }
        }

        public static bool CanAcceptData(IDropInfo pDropInfo)
        {
            if (pDropInfo?.DragInfo == null)
            {
                return false;
            }

            if (Equals(pDropInfo.DragInfo.SourceCollection, pDropInfo.TargetCollection))
            {
                return GetList(pDropInfo.TargetCollection) != null;
            }
            if (pDropInfo.DragInfo.SourceCollection is ItemCollection)
            {
                return false;
            }
            if (TestCompatibleTypes(pDropInfo.TargetCollection, pDropInfo.Data))
            {
                return !IsChildOf(pDropInfo.VisualTargetItem, pDropInfo.DragInfo.VisualSourceItem);
            }
            return false;
        }

        public static IEnumerable ExtractData(object pData)
        {
            var data = pData as IEnumerable;
            if (data != null && !(pData is string))
            {
                return data;
            }
            return Enumerable.Repeat(pData, 1);
        }

        public static IList GetList(IEnumerable pEnumerable)
        {
            var view = pEnumerable as ICollectionView;
            if (view != null)
            {
                return view.SourceCollection as IList;
            }
            return pEnumerable as IList;
        }

        protected static bool IsChildOf(UIElement pTargetItem, UIElement pSourceItem)
        {
            var lParent = ItemsControl.ItemsControlFromItemContainer(pTargetItem);

            while (lParent != null)
            {
                if (Equals(lParent, pSourceItem))
                {
                    return true;
                }

                lParent = ItemsControl.ItemsControlFromItemContainer(lParent);
            }
            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable pTarget, object pData)
        {
            if (pTarget == null)
            {
                return false;
            }

            TypeFilter lFilter =
                (lType, lObject) => (lType.IsGenericType && lType.GetGenericTypeDefinition() == typeof (IEnumerable<>));

            var lEnumerableInterfaces = pTarget.GetType().FindInterfaces(lFilter, null);
            var lEnumerableTypes = from i in lEnumerableInterfaces select i.GetGenericArguments().Single();

            var enumerableTypes = lEnumerableTypes as IList<Type> ?? lEnumerableTypes.ToList();
            if (!enumerableTypes.Any())
            {
                return pTarget is IList;
            }
            var lDataType = TypeHelper.GetCommonBaseClass(ExtractData(pData));
            return enumerableTypes.Any(t => t.IsAssignableFrom(lDataType));
        }
    }
}