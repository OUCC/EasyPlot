using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyPlot.Models
{
    public class GraphGroupModel
    {
        /// <summary>
        /// セッション中でユニークなIDです
        /// </summary>
        [JsonIgnore]
        public int Id { get; } = _idSource++;
        private static int _idSource = 0;

        public List<GraphSettingModel> Settings = new();

        public string GroupTitle { get; set; } = string.Empty;

        public bool IsWithLines { get; set; }

        public TextValueModel<string> LineWidth { get; set; } = new(string.Empty);

        public TextValueModel<string> LineType { get; set; } = new(string.Empty);

        public bool IsWithPoints { get; set; }

        public TextValueModel<string> PointsSize { get; set; } = new(string.Empty);

        public TextValueModel<string> PointsType { get; set; } = new(string.Empty);
    }
}
