namespace EasyPlot.Models;

public class RangeValueModel<T>
{
    public RangeValueModel(T defaultValue)
    {
        Start = defaultValue;
        End = defaultValue;
    }

    public bool Enabled { get; set; }

    public T Start { get; set; }

    public T End { get; set; } 
}
