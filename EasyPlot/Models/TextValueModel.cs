using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPlot.Models
{
    public class TextValueModel<T>
    {
        public TextValueModel(T defaultValue)
        {
            Value = defaultValue;
        }

        public bool Enabled { get; set; }

        public T Value { get; set; }
    }
}
