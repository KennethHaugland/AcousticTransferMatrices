using Microsoft.Xaml.Behaviors;
using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AcousticTransferMatrices.Core.Behaviors
{
    public class PlotBehavior : Behavior<Plot>
    {


        public ObservableCollection<int> SelectedItems
        {
            get { return (ObservableCollection<int>)GetValue(SelectedItemsProperty); }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<int>), typeof(PlotBehavior), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });



        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += OnAssociatedObjectMouseDown;
            AssociatedObject.PreviewMouseUp += OnAssociatedObjectMouseUp;
            AssociatedObject.PreviewMouseMove += OnAssociatedObjectMouseMove;
            // AssociatedObject.MouseLeave += OnAssociatedObjectMouseLeave;
            Update();
        }
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= OnAssociatedObjectMouseDown;
            AssociatedObject.PreviewMouseUp -= OnAssociatedObjectMouseUp;
            AssociatedObject.PreviewMouseMove -= OnAssociatedObjectMouseMove;
            // AssociatedObject.MouseLeave -= OnAssociatedObjectMouseLeave;
            base.OnDetaching();
        }

        void OnAssociatedObjectMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(AssociatedObject);
                ScreenPoint sp = new ScreenPoint(p.X, p.Y);
                var series = AssociatedObject.ActualModel.Series[0] as OxyPlot.Series.LineSeries;
                var point = series.InverseTransform(sp);

                // int NearestPoint = (int)series.GetNearestPoint(sp, false).Index;
                if (!(double.IsNaN(point.X) && double.IsNaN(point.Y)))
                {
                    SelectedArea.MaximumX = point.X;
                    SelectedArea.MaximumY = (double)AssociatedObject.RenderSize.Height;
                }
                AssociatedObject.InvalidatePlot(true);
                e.Handled = true;
            }

        }

        public RectangleAnnotation SelectedArea;

        private int StartSelection;

        void OnAssociatedObjectMouseLeave(object sender, MouseEventArgs e)
        {

            //if (SelectedArea != null)
            //{
            //    AssociatedObject.Annotations.Remove(SelectedArea);
            //    AssociatedObject.InvalidatePlot(true);

            //}

        }
        void OnAssociatedObjectMouseDown(object sender, MouseEventArgs e)
        {

            SelectedArea = new RectangleAnnotation()
            {
                Fill = Color.FromArgb(50, 50, 50, 50),
                Stroke = Colors.Transparent,
                StrokeThickness = 2,
                MinimumX = 0,
                MinimumY = 0,
                MaximumX = 1,
                MaximumY = 1
            };

            var p = e.GetPosition(AssociatedObject);
            ScreenPoint sp = new ScreenPoint(p.X, p.Y);
            var series = AssociatedObject.ActualModel.Series[0] as OxyPlot.Series.LineSeries;
            var point = series.InverseTransform(sp);
            // var valuePoint = series.Transform(p.X, p.Y);

            int NearestPoint = (int)series.GetNearestPoint(sp, false).Index;
            StartSelection = NearestPoint;
            if (!(double.IsNaN(point.X) && double.IsNaN(point.Y)))
            {
                SelectedArea.MinimumX = point.X;
                SelectedArea.MinimumY = -(double)AssociatedObject.RenderSize.Height;
            }
            AssociatedObject.Annotations.Add(SelectedArea);
            e.Handled = true;
        }

        void OnAssociatedObjectMouseUp(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is Grid)
            {
                AssociatedObject.Annotations.Remove(SelectedArea);
                var p = e.GetPosition(AssociatedObject);
                ScreenPoint sp = new ScreenPoint(p.X, p.Y);
                var series = AssociatedObject.ActualModel.Series[0] as OxyPlot.Series.LineSeries;


                int NearestPoint = (int)series.GetNearestPoint(sp, false).Index;

                ObservableCollection<int> result = new ObservableCollection<int>();
                for (int i = Math.Min(StartSelection, NearestPoint); i <= Math.Max(StartSelection, NearestPoint); i++)
                    result.Add(i);

                AssociatedObject.Annotations.Add(new RectangleAnnotation()
                {
                    Fill = Color.FromArgb(50, 255, 0, 0),
                    Stroke = Colors.Transparent,
                    StrokeThickness = 2,
                    MinimumX = SelectedArea.MinimumX,
                    MinimumY = 0,
                    MaximumX = SelectedArea.MaximumX,
                    MaximumY = 100
                });

                SetValue(SelectedItemsProperty, result);

                AssociatedObject.InvalidatePlot(true);
                e.Handled = true;
            }
        }

        void OnAssociatedObjectTextChanged(object sender, MouseEventArgs e)
        {
            Update();
        }
        void Update()
        {
            if (AssociatedObject == null) return;


        }
    }
}
