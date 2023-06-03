using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyPlot.Models;

internal class GraphSettingModel
{
    /// <summary>
    /// セッション中でユニークなIDです
    /// </summary>
    [JsonIgnore]
    public int Id { get; } = _idSource++;
    private static int _idSource = 0;

    public TextValueModel<string> Title { get; set; } = new(string.Empty);

    public bool IsFunction { get; set; } = true;

    public string FunctionText { get; set; } = string.Empty;

    public string DataFilePath { get; set; } = string.Empty;

    public RangeValueModel<string> UsingRange { get; set; } = new(string.Empty);
}
