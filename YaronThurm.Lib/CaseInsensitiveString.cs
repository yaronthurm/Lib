using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaronThurm.Lib {
    public class CaseInsensitiveString {
        private string _value;

        public CaseInsensitiveString(string value)
        {
            _value = value;
        }

        public string Value{
            get { return _value; }
        }

        public override int GetHashCode()
        {
            return _value.ToUpper().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return string.Equals(_value, ((CaseInsensitiveString)obj)._value, StringComparison.OrdinalIgnoreCase);
        }


        public static bool operator ==(CaseInsensitiveString s1, CaseInsensitiveString s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator !=(CaseInsensitiveString s1, CaseInsensitiveString s2)
        {
            return !s1.Equals(s2);
        }
        

        public static CaseInsensitiveString operator +(CaseInsensitiveString s1, CaseInsensitiveString s2)
        {
            return new CaseInsensitiveString(s1._value + s2._value);
        }

        public static CaseInsensitiveString operator +(CaseInsensitiveString s1, string s2)
        {
            return new CaseInsensitiveString(s1._value + s2);
        }

        public static CaseInsensitiveString operator +(string s1, CaseInsensitiveString s2)
        {
            return new CaseInsensitiveString(s1 + s2._value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
