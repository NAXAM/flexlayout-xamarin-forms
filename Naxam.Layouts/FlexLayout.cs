using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Naxam.Layouts
{
    [ContentProperty(nameof(Children))]
    public class FlexLayout : Layout<View>
    {
        public static BindableProperty ItemAlignProperty = BindableProperty.Create(
           nameof(ItemAlign),
           typeof(ItemAlignment),
           typeof(FlexLayout),
           ItemAlignment.Start,
           BindingMode.TwoWay
           );
        public ItemAlignment ItemAlign
        {
            get => (ItemAlignment)GetValue(ItemAlignProperty);
            set => SetValue(ItemAlignProperty, value);
        }

        public static BindableProperty RowSpacingProperty = BindableProperty.Create(
           nameof(RowSpacing),
           typeof(double),
           typeof(FlexLayout),
           0d,
           BindingMode.TwoWay
           );
        public double RowSpacing
        {
            get => (double)GetValue(RowSpacingProperty);
            set => SetValue(RowSpacingProperty, value);
        }

        public static BindableProperty ItemSpacingProperty = BindableProperty.Create(
           nameof(ItemSpacing),
           typeof(double),
           typeof(FlexLayout),
           0d,
           BindingMode.TwoWay
           );
        public double ItemSpacing
        {
            get => (double)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            int row = 0;
            double total_w = 0;
            var maxlines = CalRowSize(width, height);
            foreach (var item in Children)
            {
                View view = (View)item;
                if (view == null)
                    continue;
                var size = view.Measure(width, height, MeasureFlags.IncludeMargins);

                var w = total_w + size.Request.Width + ItemSpacing;

                double xx = 0, yy = 0;
                if (w > Width)
                {
                    w -= ItemSpacing;
                }

                if (w > Width)
                {
                    total_w = size.Request.Width + ItemSpacing;
                    row++;
                }
                else
                {
                    total_w = w;
                }
                xx = total_w - size.Request.Width - ItemSpacing;
                yy = maxlines.Take(row).Sum(d => d.MaxHeight);
                if (IsFirstColumn(maxlines, Children.IndexOf(item)))
                {
                    LayoutChildIntoBoundingRegion(view, GetRectangleFromLayoutOption(view.VerticalOptions, xx, yy, size.Request.Width, size.Request.Height, maxlines[row % maxlines.Count], row * RowSpacing, 0));
                    //continue;
                }
                else LayoutChildIntoBoundingRegion(view, GetRectangleFromLayoutOption(view.VerticalOptions, xx, yy, size.Request.Width, size.Request.Height, maxlines[row % maxlines.Count], row * RowSpacing, ItemSpacing));
            }
        }


        bool IsFirstColumn(IList<RowInfomation> lst, int position)
        { 
            if (lst == null || position < 0)
                return false;
            for (int i = 0; i < lst.Count; i++)
            {
                if (position + 1 > lst[i].ItemCount)
                {
                    position -= lst[i].ItemCount - 1;
                }
                else
                {
                    return position == 0;
                }
            }
            return false;
        }

        IList<RowInfomation> CalRowSize(double width, double height)
        {
            double total_w = 0;
            double linemax = 0;
            int itemCount = 0;
            IList<RowInfomation> lst = new List<RowInfomation>();
            foreach (var item in Children)
            {
                itemCount++;
                View view = (View)item;
                if (view == null)
                    continue;
                var size = view.Measure(width, height, MeasureFlags.IncludeMargins);
                var w = total_w + size.Request.Width + ItemSpacing;
                if (linemax <= size.Request.Height)
                {
                    linemax = size.Request.Height;
                }
                if (w > Width)
                {
                    w -= ItemSpacing;
                }
                if (w > Width)
                {
                    lst.Add(new RowInfomation
                    {
                        MaxHeight = linemax,
                        MaxWidth = total_w,
                        ItemCount = itemCount - 1
                    });
                    itemCount = 1;
                    total_w = size.Request.Width + ItemSpacing;
                    linemax = 0;
                }
                else
                {
                    total_w = w;
                }
                if (Children.LastOrDefault() == item)
                {
                    lst.Add(new RowInfomation
                    {
                        MaxHeight = linemax,
                        MaxWidth = total_w,
                        ItemCount = itemCount
                    });
                }
            }
            return lst;
        }

        Rectangle GetRectangleFromLayoutOption(
            LayoutOptions option,
            double x, double y, double w, double h, RowInfomation size, double addH, double addW)
        {
            double realY = 0;
            double realH = 0;
            double realX = 0;
            double realW = 0;
            switch (option.Alignment)
            {
                case LayoutAlignment.Center:
                    realY = y + (size.MaxHeight - h) / 2;
                    realH = h;
                    break;
                case LayoutAlignment.End:
                    realY = y + size.MaxHeight - h;
                    realH = h;
                    break;
                case LayoutAlignment.Fill:
                    realH = size.MaxHeight;
                    realY = y;
                    break;
                case LayoutAlignment.Start:
                default:
                    realY = y;
                    realH = h;
                    break;
            }
            switch (ItemAlign)
            {
                case ItemAlignment.Center:
                    realX = x + (Width - size.MaxWidth) / 2;
                    realW = w;
                    break;
                case ItemAlignment.End:
                    realX = x + (Width - size.MaxWidth);
                    realW = w;
                    break;
                case ItemAlignment.Justify:
                case ItemAlignment.Start:
                default:
                    realX = x;
                    realW = w;
                    break;
            }
            return new Rectangle(realX + addW, realY + addH, realW, realH);
        }

        class RowInfomation
        {
            public double MaxHeight { get; set; }
            public double MaxWidth { get; set; }
            public int ItemCount { get; set; }
        }

        public enum ItemAlignment
        {
            Start, Center, End, Justify
        }
    }
}
