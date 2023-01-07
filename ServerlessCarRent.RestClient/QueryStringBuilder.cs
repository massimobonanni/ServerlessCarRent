using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCarRent.RestClient
{
    public class QueryStringBuilder
    {
        private StringBuilder _innerBuilder = new StringBuilder();

        public void Append(string key, string value, bool skipIfValueIsNull = true)
        {
            if (skipIfValueIsNull && value == null)
                return;

            if (_innerBuilder.Length > 0)
                _innerBuilder.Append("&");
            _innerBuilder.Append($"{key}={value}");
        }

        public bool IsEmpty { get => _innerBuilder.Length == 0; }

        public override string ToString()
        {
            return _innerBuilder.ToString();
        }
    }
}
