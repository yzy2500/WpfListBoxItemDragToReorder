using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfListBoxItemDragToReorder
{
    public class ListBoxItemDragBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), handledEventsToo: false);
        }

        protected override void OnDetaching()
        {
            base.AssociatedObject.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
        }

        private async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement f) return;
            Panel? panel = null;
            ListBox? listBox = null;
            DependencyObject? obj = f;
            while (obj is FrameworkElement)
            {
                obj = VisualTreeHelper.GetParent(obj);
                if (obj is ListBox box) listBox = box;
                else if (panel == null) 
                {
                    if (obj is Panel p) panel = p;
                    else if (obj is FrameworkElement t) f = t;
                }
            }
            if (panel == null || listBox == null || listBox.ItemsSource is not IList list || !list.Contains(f.DataContext)) return;

            var old = f.RenderTransform;
            var z = Panel.GetZIndex(panel);
            try
            {
                Panel.SetZIndex(f, 999);
                MatrixTransform matrix = new MatrixTransform(old.Value);
                TranslateTransform translate = new TranslateTransform();
                TransformGroup group = new TransformGroup();
                group.Children.Add(matrix);
                group.Children.Add(translate);
                f.RenderTransform = group;

                var start = Mouse.GetPosition(panel);
                var end = start;
                while (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    await Task.Delay(20);
                    end = Mouse.GetPosition(panel);
                    translate.Y = end.Y - start.Y;
                }

                var data = f.DataContext;
                var index = list.IndexOf(data);
                var h = f.ActualHeight;
                var order = (int)(((index + 0.5) * h + translate.Y) / h + 0.5);
                if (index < order) order--;
                list.Remove(data);
                list.Insert(order, data);
            }
            finally
            {
                Panel.SetZIndex(f, z);
                f.RenderTransform = old;
            }
        }
    }
}
