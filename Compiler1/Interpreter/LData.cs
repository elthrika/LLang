using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler1
{
    internal interface LData
    {
        dynamic GetValue(string fname = null);
        void SetValue(LData val, string fname);
        void SetValue(dynamic val);
        string Stringify();
        string TypeName();
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

            return LNullable.GetForTypeSymbol(ts);
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

        public void SetValue(LData val, string fname)
        {
            Value = val.GetValue();
        }

        public void SetValue(dynamic val)
        {
            if (val is LData)
            {
                SetValue(val as LData, null);
            }
            else
            {
                Value = val;
            }
        }

        public string Stringify()
        {
            return Value.ToString();
        }

        public string TypeName()
        {
            return "int";
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

        public void SetValue(LData val, string fname)
        {
            Value = val.GetValue();
        }

        public void SetValue(dynamic val)
        {
            if (val is LData)
            {
                SetValue(val as LData, null);
            }
            else
            {
                Value = val;
            }
        }

        public string Stringify()
        {
            return Value.ToString();
        }

        public string TypeName()
        {
            return "float";
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

        public void SetValue(LData val, string fname)
        {
            Value = val.GetValue();
        }

        public void SetValue(dynamic val)
        {
            if (val is LData)
            {
                SetValue(val as LData, null);
            }
            else
            {
                Value = val;
            }
        }

        public string Stringify()
        {
            return Value.ToString();
        }

        public string TypeName()
        {
            return "bool";
        }
    }

    internal abstract class LNullable : LData
    {
        internal bool isNull { get; set; }

        internal LNullable(bool isnull)
        {
            isNull = isnull;
        }

        public abstract dynamic GetValue(string fname = null);
        public abstract void SetValue(LData val, string fname);
        public void SetValue(dynamic val)
        {
            if(val is LData)
            {
                SetValue(val as LData, null);
                
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public abstract string Stringify();
        public abstract string TypeName();

        internal static LNullable GetForTypeSymbol(TypeSymbol ts)
        {
            switch (ts.Kind)
            {
                case TypeSymbol.TypeKind.ARRAY:
                    return new LArray(null, true);
                case TypeSymbol.TypeKind.POINTER:
                case TypeSymbol.TypeKind.STRUCT:
                    if (ts.Name == "string") return new LString(null, true);
                    LStruct struc = new LStruct(null, ts.Name, true);
                    return struc;
            }
            throw new Exception("Not-Nullable type called Nullable? : " + ts.ToString());
        }

    }

    internal class LNull : LNullable
    {
        internal static LNull NULL = new LNull();

        private LNull() : base(true)
        {

        }

        public override dynamic GetValue(string fname = null)
        {
            return null;
        }

        public override void SetValue(LData val, string fname)
        {
            throw new NullReferenceException();
        }

        public override string Stringify()
        {
            return "null";
        }

        public override string TypeName()
        {
            return "null";
        }
    }

    internal class LString : LNullable
    {
        long Length;
        char[] _Str;

        public LString(string str, bool isnull) : base(isnull)
        {
            _Str = str.ToCharArray();
            Length = _Str.Length;

        }

        public LString(string str) : this(str, false)
        {
        }

        public override dynamic GetValue(string fname = null)
        {
            if (isNull) return null;
            return new { length = Length, _str = _Str };
        }

        public override void SetValue(LData val, string fname)
        {
            if (val is LString)
            {
                isNull = false;
                (val as LString)._Str.CopyTo(_Str, 0);
                Length = _Str.Length;
            }
            throw new ArgumentException();
        }

        public override string Stringify()
        {
            return string.Join("", _Str);
        }

        public override string TypeName()
        {
            return "string";
        }
    }

    internal class LArray : LNullable
    {
        long Length;
        LData[] _Arr;

        internal LArray(ICollection<LData> arr, bool isnull) : base(isnull)
        {
            _Arr = arr.ToArray();
            Length = _Arr == null ? 0 : _Arr.LongLength;
        }

        public LArray(ICollection<LData> arr = null) : this(arr, false)
        {
        }

        public override dynamic GetValue(string fname = null)
        {
            if (isNull) return null;

            return new { length = Length, _arr = _Arr };
        }

        public override void SetValue(LData val, string fname)
        {
            if (val is LArray)
            {
                isNull = false;
                _Arr = (val as LArray)._Arr;
                Length = _Arr.Length;
            }
            throw new ArgumentException();
        }

        public override string Stringify()
        {
            return $"[{_Arr.Select(elem => elem.ToString()).Aggregate((e1, e2) => $"{e1},{e2}")}]";
        }

        public override string TypeName()
        {
            return "list";
        }
    }

    internal class LStruct : LNullable
    {
        Dictionary<string, LData> Value;
        string typename;

        internal LStruct(Dictionary<string, TypeSymbol> fields, string typename, bool isnull) : base(isnull)//, Dictionary<string, LData> defaultvalues)
        {
            Value = new Dictionary<string, LData>();
            if (fields != null)
            {
                foreach (var f in fields.Keys)
                {
                    Value[f] = LDataMaker.GetDataFor(fields[f]);
                }
            }
            this.typename = typename;
        }

        internal LStruct(Dictionary<string, TypeSymbol> fields, TypeSymbol type) : this(fields, type.Name, false) { }
        internal LStruct(Dictionary<string, TypeSymbol> fields, string type) : this(fields, type, false) { }


        public override dynamic GetValue(string fname = null)
        {
            if (isNull) return null;

            if (fname == null) return this;

            return Value[fname];
        }

        public override void SetValue(LData val, string fname)
        {
            if (fname == null && val is LStruct)
            {
                isNull = false;
                Value = (val as LStruct).Value;
                return;
            }

            if (isNull) throw new NullReferenceException();

            Value[fname] = val;
        }

        private string StringifyPTR()
        {
            return $"{{{Value.Keys.Select(k => $"{k}: {Value[k].TypeName()}").Aggregate((e1, e2) => $"{e1}, {e2}")}}}";
        }

        public override string Stringify()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"({typename}) {{");

            foreach (var e in Value)
            {
                var fname = e.Key;
                sb.Append($"\t{fname}: ");
                string datastr;
                if(e.Value is LStruct)
                {
                    datastr = (e.Value as LStruct).StringifyPTR();
                }
                else
                {
                    datastr = e.Value.Stringify();
                }
                foreach (var line in datastr.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendLine(line);
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        public override string TypeName()
        {
            return typename;
        }
    }
}
