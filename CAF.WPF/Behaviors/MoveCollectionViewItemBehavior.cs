#region Usings

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Behaviors
{
    #region Dependencies

    

    #endregion

    public class MoveCollectionViewItemBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction",
            typeof (MoveDirection),
            typeof (MoveCollectionViewItemBehavior),
            new PropertyMetadata(MoveDirection.Down));

        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register(
            "View",
            typeof (ICollectionView),
            typeof (MoveCollectionViewItemBehavior),
            new PropertyMetadata(null));

        public MoveDirection Direction
        {
            get => (MoveDirection) GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public ICollectionView View
        {
            get => (ICollectionView) GetValue(ViewProperty);
            set => SetValue(ViewProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Click += AssociatedObject_Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Click -= AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (View == null)
            {
                return;
            }

            switch (Direction)
            {
                case MoveDirection.Up:
                {
                    View.MoveCurrentToPrevious();
                    if (View.IsCurrentBeforeFirst)
                    {
                        View.MoveCurrentToNext();
                    }
                    break;
                }
                case MoveDirection.Down:
                {
                    View.MoveCurrentToNext();
                    if (View.IsCurrentAfterLast)
                    {
                        View.MoveCurrentToPrevious();
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}