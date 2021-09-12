using AcousticTransferMatrices.Core.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AcousticTransferMatrices.Core.CustomControls
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGrid : UserControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
            BoundedUIElements = new Dictionary<string, List<UIElement>>();
        }

        static PropertyGrid()
        {
            DataContextProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(DataContextValueChanged));
        }

        private static Dictionary<string, List<UIElement>> BoundedUIElements;

        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(PropertyGrid), new FrameworkPropertyMetadata(new PropertyChangedCallback(CategoryChanged)));

        static void CategoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid PropertyGrid = (PropertyGrid)sender;
            foreach (string category in BoundedUIElements.Keys)
            {
                foreach (UIElement UIitem in BoundedUIElements[category])
                {
                    if (category == PropertyGrid.Category || string.IsNullOrEmpty(PropertyGrid.Category))
                        UIitem.Visibility = Visibility.Visible;
                    else
                        UIitem.Visibility = Visibility.Collapsed;
                }
            }
        }


        static void DataContextValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BoundedUIElements = new Dictionary<string, List<UIElement>>();
            PropertyGrid PropertyGrid = (PropertyGrid)d;
            PropertyGrid.txtDiscription.Children.Clear();
            PropertyGrid.txtProperty.Children.Clear();
            object BindedClass = e.NewValue;
            if (BindedClass is null)
                return;

            var DefaultOrder = new PropertyOrderAttribute(100);
            var DefaultCategory = new CategoryAttribute("");

            var test = from CurrentPropertyInfo in BindedClass.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                       where CurrentPropertyInfo.CustomAttributes.Count() > 0
                       select CurrentPropertyInfo;

            var f = test.Count();
            var query = from CurrentPropertyInfo in BindedClass.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                        where CurrentPropertyInfo.CustomAttributes.Count() > 0
                        let CategoryProperty = ((CategoryAttribute[])CurrentPropertyInfo.GetCustomAttributes(typeof(CategoryAttribute))).DefaultIfEmpty(DefaultCategory).First().Category.First().ToString()
                        where (CategoryProperty == PropertyGrid.Category || (PropertyGrid.Category == null))
                        let OrderProperty = ((PropertyOrderAttribute[])CurrentPropertyInfo.GetCustomAttributes(typeof(PropertyOrderAttribute))).DefaultIfEmpty(DefaultOrder).First().Order
                        orderby OrderProperty ascending
                        select new
                        {
                            CurrentPropertyInfo,
                            CategoryProperty
                        };

            foreach (var Property in query)
            {
                BindProprtyInfo(Property.CurrentPropertyInfo, Property.CategoryProperty, BindedClass, PropertyGrid);
            }
        }

        static void AddElement(PropertyGrid PropertyGrid, UIElement control, string category, bool description = false)
        {
            if (description)
                PropertyGrid.txtDiscription
                    .Children.Add(control);
            else
                PropertyGrid.txtProperty
                    .Children.Add(control);

            if (BoundedUIElements.ContainsKey(category))
                BoundedUIElements[category].Add(control);
            else
                BoundedUIElements.Add(category, new List<UIElement>() { control });
        }

        static void BindProprtyInfo(PropertyInfo propertyInfo, string Category, object currentClass, PropertyGrid propertyGrid)
        {
            string description = (((DisplayNameAttribute[])propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute))).DefaultIfEmpty(new DisplayNameAttribute(propertyInfo.Name))).First().DisplayName;

            TextBlock Description = CreateNewTextBlock(description);
            Description.Foreground = Brushes.Black;
            AddElement(propertyGrid, Description, Category, true);

            TextBoxTypeAttribute[] textBoxAttributes = ((TextBoxTypeAttribute[])propertyInfo.GetCustomAttributes(typeof(TextBoxTypeAttribute)));
            if (textBoxAttributes.Count() == 0)
            {
                TextBox Value = CreateNewTextBox(propertyInfo.Name, currentClass);
                Value.Foreground = Brushes.Black;
                if (!(propertyInfo.CanWrite))
                    Value.IsReadOnly = true;

                AddElement(propertyGrid, Value, Category);
            }
            else
            {
                if (textBoxAttributes.First().TextBoxType == TextBoxType.ReadOnly)
                {
                    TextBlock Value = CreateNewTextBlock(propertyInfo.GetValue(currentClass).ToString());
                    Value.Foreground = Brushes.Black;
                    Value.HorizontalAlignment = HorizontalAlignment.Left;
                    AddElement(propertyGrid, Value, Category);
                }
                else if (textBoxAttributes.First().TextBoxType == TextBoxType.Masked)
                {
                    PasswordBox Value = CreateNewPasswordBox(propertyInfo.Name, currentClass);
                    Value.Foreground = Brushes.Black;
                    AddElement(propertyGrid, Value, Category);

                    if (!(propertyInfo.GetValue(currentClass) is null))
                    {
                        foreach (char item in propertyInfo.GetValue(currentClass).ToString())
                        {
                            Value.Password += item;
                        }
                    }
                }
            }

        }

        static TextBlock CreateNewTextBlock(string description)
        {
            TextBlock txt = new TextBlock
            {
                Padding = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 10),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = description
            };
            return txt;
        }

        static TextBox CreateNewTextBox(string propertyName, object currentClass)
        {
            TextBox txt = new TextBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                VerticalAlignment = VerticalAlignment.Center
            };

            Binding bind = new Binding()
            {
                Path = new PropertyPath(propertyName),
                Source = currentClass,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            if (propertyName == "Thickness")
            {
                bind.Converter = new ValueConverters.MeterToMillimeterConverter(); 
            }

            txt.SetBinding(TextBox.TextProperty, bind);
            return txt;
        }


        static PasswordBox CreateNewPasswordBox(string propertyName, object currentClass)
        {
            PasswordBox txt = new PasswordBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                VerticalAlignment = VerticalAlignment.Center
            };

            Binding bind = new Binding()
            {
                Path = new PropertyPath(propertyName),
                Source = currentClass,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            txt.SetBinding(PasswordBoxAssistant.BoundPassword, bind);
            txt.SetValue(PasswordBoxAssistant.BindPassword, true);
            return txt;
        }
    }
}

