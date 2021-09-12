using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
namespace AcousticTransferMatrices.Core.AttatchedProperties
{
    public class TreeViewSearch
    {

        private static List<TreeViewHelperClass> TreeViewHelper = new List<TreeViewHelperClass>();

      //  private static TreeView TreeViewControl;
        #region "SearchTextDP"

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.RegisterAttached("SearchText", typeof(string), typeof(TreeViewSearch), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSearchTextChanged)));
        public static string GetSearchText(DependencyObject sender)
        {
            return (string)sender.GetValue(SearchTextProperty);
        }
        public static void SetSearchText(DependencyObject sender, string SearchText)
        {
            sender.SetValue(SearchTextProperty, SearchText);
        }

        public static void OnSearchTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string TempSearchString = (string)sender.GetValue(SearchTextProperty);

            //Find the TreeView by navigating the VisualTree
            TreeView TreeViewControl = FindTheTreeVIewControl(sender);


            if (TreeViewControl != null && TreeViewControl.Items.Count != 0 && !string.IsNullOrEmpty(TempSearchString.Trim()))
            {
                // Setting all TreeViewItems to visible, and at the same time ensure that all items 
                // in the TreeView is loaded
                ApplyActionToAllTreeViewItems(itemsControl =>
                {
                    itemsControl.IsExpanded = true;
                    itemsControl.Visibility = Visibility.Visible;
                    DispatcherHelper.WaitForPriority(DispatcherPriority.ContextIdle);
                }, TreeViewControl);

                //All items are loaded into the TreeView so we can load all TreeViewITems and binded class instances 
                // into a local ViewModel called TreeViewHelper
                CreateInternalViewModelFilter(TreeViewControl);

                //The first instance is a dummy that is not connected to the TreeView, but can initiate the Search
                TreeViewHelper[0].ApplyCriteria(TempSearchString, new Stack<TreeViewHelperClass>());

            }
            else
            {
                //No search, just make sure everything is reset
                ApplyActionToAllTreeViewItems(itemsControl =>
                {
                    itemsControl.Visibility = Visibility.Visible;
                    DispatcherHelper.WaitForPriority(DispatcherPriority.ContextIdle);
                }, TreeViewControl);
            }
        }
        #endregion

        #region "WPF DoEvents"
        internal sealed class DispatcherHelper
        {
            private DispatcherHelper()
            {
            }
            /// <summary>
            /// This method allows developer to call a non-blocking wait. When calling WaitForPriority, the developer is certain
            /// that all dispatcher operations with priority higher than the one passed as a parameter will have been executed
            /// by the time the line of code that follows it is reached.
            /// Similar to VB's DoEvents.
            /// Keep in mind that this call does not guarantee that all operations at the priority passed will have been executed,
            /// only operations with priority higher than the one passed. In practice it waits for some (and sometimes all) 
            /// operations at the priority passed, but this is not guaranteed.
            /// </summary>
            /// <param name="priority">Priority below the one we want to wait for before the next line of code is executed.</param>
            static internal void WaitForPriority(DispatcherPriority priority)
            {
                DispatcherFrame frame = new DispatcherFrame();
                DispatcherOperation dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
                Dispatcher.PushFrame(frame);
                if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
                {
                    dispatcherOperation.Abort();
                }
            }

            private static object ExitFrameOperation(object obj)
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            }
        }
        #endregion

        #region "TreeViewActionCommand"
        /// <summary>
        /// Helper method that executes an action for every container in the hierarchy that starts with 
        /// the ItemsControl passed as a parameter. I'm passing an ItemsControl so that this method can be
        /// called with both TreeView and TreeViewItem as parameters.
        /// </summary>
        /// <param name="itemAction">Action to be executed for every item.</param>
        /// <param name="itemsControl">ItemsControl (TreeView or TreeViewItem) at the top of the hierarchy.</param>
        private static void ApplyActionToAllTreeViewItems(Action<TreeViewItem> itemAction, ItemsControl itemsControl)
        {
            Stack<ItemsControl> itemsControlStack = new Stack<ItemsControl>();

            itemsControlStack.Push(itemsControl);

            while (itemsControlStack.Count != 0)
            {
                ItemsControl currentItem = itemsControlStack.Pop() as ItemsControl;
                TreeViewItem currentTreeViewItem = currentItem as TreeViewItem;

                if (currentTreeViewItem != null)
                {
                    itemAction(currentTreeViewItem);
                }
                if (currentItem != null)
                {
                    // this handles the scenario where some TreeViewItems are already collapsed
                    foreach (object dataItem in currentItem.Items)
                    {
                        ItemsControl childElement = (ItemsControl)currentItem.ItemContainerGenerator.ContainerFromItem(dataItem);
                        itemsControlStack.Push(childElement);
                    }
                }
            }
        }
        #endregion

        #region "GenerateTreeViewCopy"
        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/a2988ae8-e7b8-4a62-a34f-b851aaf13886/windows-presentation-foundation-faq?forum=wpf#expand_treeview
        private static void CreateInternalViewModelFilter(ItemsControl parentContainer)
        {
            if ((parentContainer) is TreeView)
            {
                TreeViewHelper = new List<TreeViewHelperClass>();
            }
            TreeViewHelperClass EmptyFirstTreeViewItem = new TreeViewHelperClass();
            TreeViewHelper.Add(EmptyFirstTreeViewItem);

            foreach (Object item in parentContainer.Items)
            {
                TreeViewHelperClass TreeViewItemHelperContainer = new TreeViewHelperClass();

                TreeViewItemHelperContainer.BindedClass = item;
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                TreeViewItemHelperContainer.CurrentTreeViewItem = currentContainer;

                EmptyFirstTreeViewItem.Children.Add(TreeViewItemHelperContainer);


                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        //The code should never reach this part. 

                        // If the sub containers of current item is not ready, we need to wait until 
                        // they are generated. 
                        DispatcherHelper.WaitForPriority(DispatcherPriority.Background);
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate
                        {

                            CreateInternalViewModelFilter(currentContainer, ref TreeViewItemHelperContainer);
                        };

                    }
                    else
                    {
                        // If the sub containers of current item is ready, we can directly go to the next 
                        // iteration to expand them. 
                        CreateInternalViewModelFilter(currentContainer, ref TreeViewItemHelperContainer);
                    }
                }

            }
        }

        private static void CreateInternalViewModelFilter(ItemsControl parentContainer, ref TreeViewHelperClass ParentTreeItem)
        {

            foreach (Object item in parentContainer.Items)
            {
                TreeViewHelperClass TreeViewItemHelperContainer = new TreeViewHelperClass();

                TreeViewItemHelperContainer.BindedClass = item;
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                TreeViewItemHelperContainer.CurrentTreeViewItem = currentContainer;
                ParentTreeItem.Children.Add(TreeViewItemHelperContainer);

                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    // Expand the current item. 
                    currentContainer.IsExpanded = true;

                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        // If the sub containers of current item is not ready, we need to wait until 
                        // they are generated. 
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate { CreateInternalViewModelFilter(currentContainer, ref TreeViewItemHelperContainer); };
                    }
                    else
                    {
                        // If the sub containers of current item is ready, we can directly go to the next 
                        // iteration to expand them. 
                        CreateInternalViewModelFilter(currentContainer, ref TreeViewItemHelperContainer);
                    }
                }

            }
        }
        #endregion

        private static TreeView FindTheTreeVIewControl(DependencyObject dep)
        {
            //Stepping through the visual tree
            while ((dep != null) && !(dep is TreeView))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                return null;
            }
            else
            {
                return (TreeView)dep;
            }
        }
    }

}
