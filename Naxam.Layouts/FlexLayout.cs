using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using static Xamarin.Forms.Grid;

namespace Naxam.Layouts
{
    [ContentProperty(nameof(Children))]
    public class FlexLayout : Layout<View>
    {
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            int row = 0;
            double total_w = 0;
            var maxlines = CalMaxHeight(width, height);
            foreach (var item in Children.Where(d=>d.IsVisible==true))
            {
                View view = (View)item;
                if (view == null)
                    continue;
                var size = view.Measure(width, height, MeasureFlags.IncludeMargins);

                var w = total_w + size.Request.Width;

                double xx = 0, yy = 0;
                if (w > Width)
                {
                    total_w = size.Request.Width;
                    row++;
                }
                else
                {
                    total_w = w;
                }
                xx = total_w - size.Request.Width;
                yy = maxlines.Take(row).Sum(d => d);
                LayoutChildIntoBoundingRegion(view, GetRectangleFromLayoutOption(view.VerticalOptions, xx, yy, size.Request.Width, size.Request.Height, maxlines[row % maxlines.Count]));
            }
        }

        IList<double> CalMaxHeight(double width, double height)
        {
            double total_w = 0;
            IList<double> maxlines = new List<double>();
            List<double> lst = new List<double>();
            foreach (var item in Children.Where(d => d.IsVisible == true))
            {
                View view = (View)item;
                if (view == null)
                    continue;
                var size = view.Measure(width, height, MeasureFlags.IncludeMargins);
                var w = total_w + size.Request.Width;
                lst.Add(size.Request.Height);
                if (w > Width)
                {
                    total_w = size.Request.Width;
                    lst.RemoveAt(lst.Count - 1);
                    maxlines.Add(lst.Max());
                    lst.Clear();
                    lst.Add(size.Request.Height);
                }
                else
                {
                    total_w = w;
                }

                if (Children.LastOrDefault() == item)
                {
                    maxlines.Add(lst.Max());
                }
            }
            return maxlines;
        }

        Rectangle GetRectangleFromLayoutOption(LayoutOptions option, double x, double y, double w, double h, double maxHeight)
        {
            switch (option.Alignment)
            {
                case LayoutAlignment.Center:
                    return new Rectangle(x, y + (maxHeight - h) / 2, w, h);
                case LayoutAlignment.End:
                    return new Rectangle(x, y + maxHeight - h, w, h);
                case LayoutAlignment.Fill:
                    return new Rectangle(x, y, w, maxHeight);
                case LayoutAlignment.Start:
                default:
                    return new Rectangle(x, y, w, h);
            }
        }
    }
}
