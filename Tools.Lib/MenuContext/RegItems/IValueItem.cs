namespace Tools.Lib.MenuContext.RegItems
{
    public interface IValueItem { }

    public interface IValueItem<T> : IValueItem
    {
        T ItemValue { get; set; }
    }
}
