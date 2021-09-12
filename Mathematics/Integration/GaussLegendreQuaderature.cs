using System;
using System.Collections.Generic;

namespace Mathematics.Integration
{
    [Serializable]
    public class GaussLegendreQuaderature
    {

        public int Number { get; set; }
        public List<QuaderatureItem> Items { get; set; } = new List<QuaderatureItem>();
    }
}
