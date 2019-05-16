using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    internal interface LData
    {
        dynamic GetValue(string fname = null);
        void SetValue(dynamic val, string fname = null);
    }

    internal static class LDataMaker
    {
        public static LData GetDataFor(TypeSymbol ts)
        {
            if(ts.Kind == TypeSymbol.TypeKind.PRIMITIVE)
            {
                switch (ts.Name)
                {
                    case "int":
                        return new LInt(0);
                    case "float":
                        return new LFloat(0);
                    case "bool":
                        return new LBool(false);
                    default:
                        throw new ArgumentException("void does not have a value!");
                }
            }

            return new LNull();
        }
    }

    internal class LInt : LData
    {
        long Value;

        internal LInt(long val)
        {
            Value = val;
        }

        public dynamic GetValue(string fname = null)
        {
            return Value;
        }

        public void SetValue(dynamic val, string fname = null)
        {
            Value = val;
        }
    }

    internal class LFloat : LData
    {
        double Value;

        internal LFloat(double val)
        {
            Value = val;
        }

        public dynamic GetValue(string fname = null)
        {
            return Value;
        }

        public void SetValue(dynamic val, string fname = null)
        {
            Value = val;
        }
    }

    internal class LString : LData
    {
        long Length;
        char[] _Str;

        public LString(string str)
        {
            _Str = str.ToCharArray();
            Length = _Str.Length;
        }

        public dynamic GetValue(string fname = null)
        {
            return new { length = Length, _str = _Str };
        }

        public void SetValue(dynamic val, string fname = null)
        {
            _Str = val;
            Length = _Str.Length;
        }
    }

    internal class LBool : LData
    {
        bool Value;

        internal LBool(bool val)
        {
            Value = val;
        }

        public dynamic GetValue(string fname = null)
        {
            return Value;
        }

        public void SetValue(dynamic val, string fname = null)
        {
            Value = val;
        }
    }

    internal class LArray : LData
    {
        long Length;
        LData[] _Arr;

        public LArray(ICollection<LData> arr = null)
        {
            _Arr = arr.ToArray();
            Length = _Arr == null ? 0 : _Arr.LongLength;
        }

        public dynamic GetValue(string fname = null)
        {
            return new { length = Length, _arr = _Arr };
        }

        public void SetValue(dynamic val, string fname = null)
        {
            _Arr = val;
            Length = _Arr.Length;
        }

    }

    internal class LNull : LData
    {
        public dynamic GetValue(string fname = null)
        {
            return null;
        }

        public void SetValue(dynamic val, string fname = null)
        {
        }

    }

    internal class LStruct : LData
    {
        Dictionary<string, LData> Value;

        internal LStruct(Dictionary<string, TypeSymbol> fields)//, Dictionary<string, LData> defaultvalues)
        {
            Value = new Dictionary<string, LData>();
            foreach (var f in fields.Keys)
            {
                //if (defaultvalues.ContainsKey(f))
                //{
                //    Value[f] = defaultvalues[f];
                //}
                //else
                //{
                //    Value[f] = LDataMaker.GetDataFor(fields[f]);
                //}
                Value[f] = LDataMaker.GetDataFor(fields[f]);
            }
        }

        public dynamic GetValue(string fname = null)
        {
            return Value[fname];
        }

        public void SetValue(dynamic val, string fname = null)
        {
            Value[fname] = val;
        }

    }
}
