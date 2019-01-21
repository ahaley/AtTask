using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ahaley.AtTask
{
    class AtTaskDateConverter : IsoDateTimeConverter
    {
        public AtTaskDateConverter()
        {
            base.DateTimeFormat = AtTaskExtentionMethods.AtTaskDateTimeFormat;
        }
    }
}
