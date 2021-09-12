using System;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

namespace AcousticTransferMatrices.Core.CustomControls.TreeViewForms
{
    public class TreeViewForms
    {

        private TreeViewItem _item;

        public TreeViewForms(TreeViewItem item)
        {
            _item = item;

            VisibilityDescriptor.AddValueChanged(_item, VisibilityChanged);

            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(_item);
            ic.ItemContainerGenerator.ItemsChanged += OnItemsChangedItemContainerGenerator;

            _item.SetValue(IsLastOneProperty, ic.ItemContainerGenerator.IndexFromContainer(_item) == ic.Items.Count - 1);
        }

        private void OnItemsChangedItemContainerGenerator(object sender, ItemsChangedEventArgs e)
        {
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(_item);

            if (ic != null)
            {
                _item.SetValue(IsLastOneProperty, ic.ItemContainerGenerator.IndexFromContainer(_item) == ic.Items.Count - 1);
            }
        }


        private static DependencyPropertyDescriptor VisibilityDescriptor = DependencyPropertyDescriptor.FromProperty(TreeViewItem.VisibilityProperty, typeof(TreeViewItem));

        private void VisibilityChanged(object sender, EventArgs e)
        {
            if ((sender) is TreeView)
            {
                return;
            }

            if (((ItemsControl)_item).Visibility == Visibility.Collapsed)
            {
                ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(_item);
                int Index = ic.ItemContainerGenerator.IndexFromContainer(_item);

                if (Index != 0 && (bool)_item.GetValue(IsLastOneProperty))
                {
                    ((TreeViewItem)ic.ItemContainerGenerator.ContainerFromIndex(Index - 1)).SetValue(IsLastOneProperty, true);
                }
            }
            else
            {
                ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(_item);
                int Index = ic.ItemContainerGenerator.IndexFromContainer(_item);

                if (Index != 0)
                {
                    ((TreeViewItem)ic.ItemContainerGenerator.ContainerFromIndex(Index - 1)).SetValue(IsLastOneProperty, false);
                }
            }
        }

        private void Detach()
        {
            if (_item != null)
            {
                ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(_item);
                ic.ItemContainerGenerator.ItemsChanged -= OnItemsChangedItemContainerGenerator;

                VisibilityDescriptor.RemoveValueChanged(_item, VisibilityChanged);
            }
        }

        #region "UseExtenderDP"

        public static DependencyProperty UseExtenderProperty = DependencyProperty.RegisterAttached("UseExtender", typeof(bool), typeof(TreeViewForms),
            new PropertyMetadata(false, new PropertyChangedCallback(OnChangedUseExtender)));
        public static bool GetUseExtender(DependencyObject sender)
        {
            return Convert.ToBoolean(sender.GetValue(UseExtenderProperty));
        }
        public static void SetUseExtender(DependencyObject sender, bool useExtender)
        {
            sender.SetValue(UseExtenderProperty, useExtender);
        }

        private static void OnChangedUseExtender(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                if (Convert.ToBoolean(e.NewValue))
                {
                    if (object.ReferenceEquals(item.ReadLocalValue(ItemExtenderProperty), DependencyProperty.UnsetValue))
                    {
                        TreeViewForms extender = new TreeViewForms(item);
                        item.SetValue(ItemExtenderProperty, extender);
                    }
                }
                else
                {
                    if (!object.ReferenceEquals(item.ReadLocalValue(ItemExtenderProperty), DependencyProperty.UnsetValue))
                    {
                        TreeViewForms extender = (TreeViewForms)item.ReadLocalValue(ItemExtenderProperty);
                        extender.Detach();
                        item.SetValue(ItemExtenderProperty, DependencyProperty.UnsetValue);
                    }
                }
            }
        }
        #endregion


        public static DependencyProperty ItemExtenderProperty = DependencyProperty.RegisterAttached("ItemExtender", typeof(TreeViewForms), typeof(TreeViewForms));
        #region "IsLastOneDP"

        public static DependencyProperty IsLastOneProperty = DependencyProperty.RegisterAttached("IsLastOne", typeof(bool), typeof(TreeViewForms));
        public static bool GetIsLastOne(DependencyObject sender)
        {
            return Convert.ToBoolean(sender.GetValue(IsLastOneProperty));
        }
        public static void SetIsLastOne(DependencyObject sender, bool isLastOne)
        {
            sender.SetValue(IsLastOneProperty, isLastOne);
        }
        #endregion
    }
}


