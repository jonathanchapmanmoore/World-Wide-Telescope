using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Renci.Wwt.Core
{
    public abstract class DataItem
    {
        protected DataItem()
        {

        }

        public abstract void WriteHeaderTo(TextWriter sw);

        public abstract void WriteDataTo(TextWriter sw);
        
        public abstract void ReadHeaderFrom(TextReader sr);

        public abstract void ReadDataFrom(TextReader sr);

        protected IEnumerable<string> GetCsvFields(string data)
        {
            var delimeter = ',';
            var enclosedChar = "\"";

            var fields = data.Split(delimeter);

            var enclosedField = false;
            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                if (field.StartsWith(enclosedChar))
                {
                    enclosedField = true;
                }

                if (enclosedField && field.EndsWith(enclosedChar))
                {
                    enclosedField = false;
                    sb.Append(field);
                    yield return sb.ToString();
                    sb.Length = 0;
                    continue;
                }

                if (enclosedField)
                {
                    sb.Append(field);
                    continue;
                }

                yield return field;                
            }
        }
    }
}
