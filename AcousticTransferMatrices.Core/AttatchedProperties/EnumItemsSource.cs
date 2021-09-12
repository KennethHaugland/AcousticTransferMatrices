using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.Linq;

namespace AcousticTransferMatrices.Core.AttatchedProperties
{

    public class EnumItemsSource
    {
        private static Dictionary<ComboBox, ComboItems> RegisteredComboBoxes = new Dictionary<ComboBox, ComboItems>();


        public static readonly DependencyProperty EnumItemsSourceProperty = DependencyProperty.RegisterAttached("EnumItemsSource", typeof(object), typeof(EnumItemsSource), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEnumItemsSourceChanged)));
        public static object GetEnumItemsSource(DependencyObject sender)
        {
            return ((object)sender.GetValue(EnumItemsSourceProperty));
        }

        public static void SetEnumItemsSource(DependencyObject sender, object EnumSource)
        {
            sender.SetValue(EnumItemsSourceProperty, EnumSource);
        }

        private static void OnEnumItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox current = (ComboBox)sender;

            List<ComboBox> ItemsToRemove = new List<ComboBox>();

            foreach (ComboBox item in RegisteredComboBoxes.Keys)
            {
                // Remove items that does not have a visual parent
                if (VisualTreeHelper.GetParent(item) == null)
                    ItemsToRemove.Add(item);

            }
            foreach (ComboBox item in ItemsToRemove)
            {
                RegisteredComboBoxes[(ComboBox)item].localCombo.SelectionChanged -= SelectedEnumValueChanged;
                _ = RegisteredComboBoxes.Remove(item);
            }

            // First time registration and visual parent is not null
            if (!RegisteredComboBoxes.ContainsKey((ComboBox)sender) && (VisualTreeHelper.GetParent((ComboBox)sender) != null))
            {
                RegisteredComboBoxes[(ComboBox)sender] = new ComboItems();
                RegisteredComboBoxes[(ComboBox)sender].localCombo = current;
                RegisteredComboBoxes[(ComboBox)sender].localCombo.SelectionChanged -= SelectedEnumValueChanged;
                RegisteredComboBoxes[(ComboBox)sender].localCombo.SelectionChanged += SelectedEnumValueChanged;
                //Store the set enum locally
                Enum TempEnum = (Enum)sender.GetValue(EnumItemsSourceProperty);


                if (RegisteredComboBoxes[current].ComboEnumList == null || !RegisteredComboBoxes[current].ComboEnumList.Contains(TempEnum))
                {

                    //Get all possible values for the enum type
                    Array EnumValues = Enum.GetValues(TempEnum.GetType());

                    // Loop trough them and store the description 
                    // and the Enum type in two separate lists
                    foreach (Enum EnumValue in EnumValues)
                    {
                        RegisteredComboBoxes[current].ComboNameList.Add(GetDescription(EnumValue));
                        RegisteredComboBoxes[current].ComboEnumList.Add(EnumValue);
                    }
                    // Set ItemsSource of the ComboBox
                    current.ItemsSource = RegisteredComboBoxes[current].ComboNameList;
                }
                current.SelectedIndex = RegisteredComboBoxes[current].ComboEnumList.IndexOf(TempEnum);
            }
        }


        private static void SelectedEnumValueChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            SetEnumItemsSource((DependencyObject)sender, (object)RegisteredComboBoxes[s].ComboEnumList[s.SelectedIndex]);
        }

        private static string GetDescription(Enum value)
        {
            // Get information on the enum element
            FieldInfo fi = value.GetType().GetField(value.ToString());
            // Get description for enum element
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                // DescriptionAttribute exists - return that
                return attributes[0].Description;
            }
            // No Description set - return enum element name
            return value.ToString();
        }

        internal class ComboItems
        {
            public ComboBox localCombo;
            public List<string> ComboNameList = new List<string>();
            public List<Enum> ComboEnumList = new List<Enum>();

        }
    }
}

