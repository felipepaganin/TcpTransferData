using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusFuncion
{
    [Serializable]
    class SampleObject
    {
        public DateTime Date { get; set; }
        public string IP { get; set; }
        public string URL { get; set; }
    }
}
