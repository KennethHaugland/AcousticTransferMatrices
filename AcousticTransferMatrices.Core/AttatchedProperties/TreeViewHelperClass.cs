using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AcousticTransferMatrices.Core.AttatchedProperties
{

    public class TreeViewHelperClass : System.ComponentModel.INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private TreeViewItem pCurrentTreeViewItem;
        public TreeViewItem CurrentTreeViewItem
        {
            get { return pCurrentTreeViewItem; }
            set { pCurrentTreeViewItem = value; }
        }

        private object pBindedClass;
        public object BindedClass
        {
            get { return pBindedClass; }
            set { pBindedClass = value; }
        }

        private List<TreeViewHelperClass> pChildren = new List<TreeViewHelperClass>();
        public List<TreeViewHelperClass> Children
        {
            get { return pChildren; }
            set { pChildren = value; }
        }

        private bool FindString(object obj, string SearchString)
        {

            if (string.IsNullOrEmpty(SearchString))
            {
                return true;
            }

            if (obj == null)
                return true;

            foreach (System.Reflection.PropertyInfo p in obj.GetType().GetProperties())
            {
                if (p.PropertyType == typeof(string))
                {
                    string value = (string)p.GetValue(obj);
                    if (value.ToLower().Contains(SearchString.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private bool match = true;
        private bool IsCriteriaMatched(string criteria)
        {
            return FindString(BindedClass, criteria);
        }

        public void ApplyCriteria(string criteria, Stack<TreeViewHelperClass> ancestors)
        {
            if (IsCriteriaMatched(criteria))
            {
                IsMatch = true;

                foreach (var ancestor in ancestors)
                {
                    ancestor.IsMatch = true;
                    //    ancestor.IsExpanded = Not String.IsNullOrWhiteSpace(criteria)
                }

            }
            else
            {
                IsMatch = false;
            }

            //if (this.BindedClass != null)
            //{
            //    var s = ((Person)this.pBindedClass).Name.ToString();
            //    Debug.WriteLine(s.ToString());
            //}


            ancestors.Push(this);
            // and then just touch me
            foreach (var child in Children)
            {
                child.ApplyCriteria(criteria, ancestors);
            }
            ancestors.Pop();
        }

        public bool IsMatch
        {
            get { return match; }
            set
            {
                //if (match == value)
                //{

                //    return;
                //}
                match = value;

                if (CurrentTreeViewItem != null)
                {
                    if (match)
                    {
                        CurrentTreeViewItem.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        CurrentTreeViewItem.Visibility = Visibility.Collapsed;
                    }
                }
                OnPropertyChanged("IsMatch");
            }
        }

        public bool IsLeaf
        {
            get { return (Children.Count == 0); }
        }
    }
}

