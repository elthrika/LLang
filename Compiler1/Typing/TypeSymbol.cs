using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class TypeSymbol : ICloneable
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public enum TypeKind
        {
            PRIMITIVE,
            ARRAY,
            STRUCT,
            FUNCTION,
            POINTER,
            ENUM,
            INFER
        }

        public static int WORD_SIZE = 8;

        public static readonly TypeSymbol INT_SYMBOL = new TypeSymbol("int", TypeKind.PRIMITIVE, WORD_SIZE); // WORD_SIZE byte size integer
        public static readonly TypeSymbol FLOAT_SYMBOL = new TypeSymbol("float", TypeKind.PRIMITIVE, WORD_SIZE); // WORD_SIZE byte size floatingpoint number
        public static readonly TypeSymbol VOID_SYMBOL = new TypeSymbol("void", TypeKind.PRIMITIVE, 0); // 0 size void type
        public static readonly TypeSymbol BOOL_SYMBOL = new TypeSymbol("bool", TypeKind.PRIMITIVE, 1); // 1 byte bool
        public static readonly TypeSymbol STRING_SYMBOL = new TypeSymbol("string", TypeKind.STRUCT, WORD_SIZE * 2, new Dictionary<string, (TypeSymbol, int)>(2) { { "length", (INT_SYMBOL, 0) }, { "_str", (POINTER_SYMBOL(INT_SYMBOL), WORD_SIZE) } }); // { int length, [char] _str }

        public static TypeSymbol POINTER_SYMBOL(TypeSymbol type)
            => new TypeSymbol(type.Name, TypeKind.POINTER, WORD_SIZE, type);

        public static TypeSymbol INFER_SYMOBOL(string name) 
            => new TypeSymbol(name, TypeKind.INFER, 0); // Type to be determined, name=typename in source

        public static TypeSymbol INFER_FROM_SYMBOL(string name, ICollection<TypeSymbol> options)
            => new TypeSymbol(name, TypeKind.INFER, options);

        public static TypeSymbol ARRAY_SYMBOL(TypeSymbol arraytype) 
            => new TypeSymbol("["+arraytype.Name+"]", TypeKind.ARRAY, WORD_SIZE * 2, arraytype, null, null, new Dictionary<string, (TypeSymbol, int)>(2) { { "length", (INT_SYMBOL, 0) }, { "_arr", (POINTER_SYMBOL(arraytype), WORD_SIZE) } }, null); // { int length, T* array }

        public static TypeSymbol FUNCTION_SYMBOL(string funname, string typename, TypeSymbol rettype, List<TypeSymbol> paramtypes)
            => new TypeSymbol(funname, typename, TypeKind.FUNCTION, rettype, paramtypes);

        public static TypeSymbol STRUCT_SYMBOL(string name, int length, Dictionary<string, (TypeSymbol, int)> fields)
            => new TypeSymbol(name, TypeKind.STRUCT, length, fields);

        public static TypeSymbol ENUM_SYMBOL(string name, Dictionary<string, long> items)
            => new TypeSymbol(name, TypeKind.ENUM, items);

        //@TODO: Maybe make this a Scope, so that types can exist on certain levels, and not only globally
        public readonly string Name;
        public readonly string FunctionName;
        public readonly TypeKind Kind;
        public readonly int Length;
        public readonly TypeSymbol ArrayType;
        public readonly TypeSymbol ReturnType;
        public readonly List<TypeSymbol> ParameterTypes;
        public readonly Dictionary<string, (TypeSymbol, int)> Fields;
        //public readonly List<int> Offsets;
        public readonly Dictionary<string, long> EnumItems;
        public readonly ICollection<TypeSymbol> inferOptions;

        public TypeSymbol(string name, TypeKind kind, int length, TypeSymbol arrayof, TypeSymbol rettype, List<TypeSymbol> parameters, Dictionary<string, (TypeSymbol, int)> fields, ICollection<TypeSymbol> options)
        {
            Name = name;
            Kind = kind;
            ArrayType = arrayof;
            ReturnType = rettype;
            ParameterTypes = parameters;
            Fields = fields;
            Length = length;
            inferOptions = options;
        }

        public TypeSymbol(string name, TypeKind kind, int len) : this(name, kind, len, null, null, null, null, null) { }
        public TypeSymbol(string name, TypeKind kind, int len, TypeSymbol arrayof) : this(name, kind, len, arrayof, null, null, null, null) { }
        public TypeSymbol(string name, TypeKind kind, int len, Dictionary<string, (TypeSymbol, int)> fields) : this(name, kind, len, null, null, null, fields, null) { }
        public TypeSymbol(string name, TypeKind kind, ICollection<TypeSymbol> options) : this(name, kind, 0, null, null, null, null, options) { }
        public TypeSymbol(string name, TypeKind kind, Dictionary<string, long> items) : this(name, kind, items.Count * WORD_SIZE)
        {
            EnumItems = items;
        }
        public TypeSymbol(string funname, string typename, TypeKind kind, TypeSymbol rettype, List<TypeSymbol> parameters) : this(typename, kind, WORD_SIZE, null, rettype, parameters, null, null)
        {
            if (Kind == TypeKind.FUNCTION)
            {
                FunctionName = MakeFunctionName(funname, parameters);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Match(TypeSymbol t2)
        {
            return Match(this, t2);
        }

        public static bool Match(TypeSymbol t1, TypeSymbol t2)
        {
            if (t1 == null || t2 == null) return false;

            if (t1.Equals(t2) || t1.Name == t2.Name) return true;
            /*
             * if t1 & t2 are functions, returntype and parameters need to be the same for equality
             * float matches int, int doesnt match float
             * void/null matches non-primitive
             * structs only match the same struct
            */
            if (t1.Equals(FLOAT_SYMBOL) && t2.Equals(INT_SYMBOL)) return true;
            if ((t1.Kind != TypeKind.PRIMITIVE && t2.Equals(VOID_SYMBOL)) || (t2.Kind != TypeKind.PRIMITIVE && t1.Equals(VOID_SYMBOL))) return true;
            if (t1.Kind == TypeKind.FUNCTION && t2.Kind == TypeKind.FUNCTION && t2.ReturnType.Equals(t1.ReturnType) && t2.ParameterTypes.SequenceEqual(t1.ParameterTypes)) return true;
            
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if(obj is TypeSymbol)
            {
                TypeSymbol ts = (TypeSymbol)obj;
                if (Name == ts.Name) return true;
            }
            return base.Equals(obj);
        }

        public object Clone()
        {
            List<TypeSymbol> parametertypes = null;
            Dictionary<string, (TypeSymbol, int)> fields = null;
            List<TypeSymbol> options = null;

            if (ParameterTypes != null)
            {
                parametertypes = new List<TypeSymbol>(ParameterTypes.Count);
                foreach (TypeSymbol s in ParameterTypes)
                {
                    parametertypes.Add((TypeSymbol)s.Clone());
                }
            }
            if (Fields != null)
            {
                fields = new Dictionary<string, (TypeSymbol, int)>(Fields.Count);
                foreach (KeyValuePair<string, (TypeSymbol, int)> s in Fields)
                {
                    fields.Add((string)s.Key.Clone(), ((TypeSymbol)s.Value.Item1.Clone(), s.Value.Item2));
                }
            }
            if(inferOptions != null)
            {
                options = new List<TypeSymbol>(inferOptions.Count);
                foreach (var s in inferOptions)
                {
                    options.Add((TypeSymbol)s.Clone());
                }
            }

            return new TypeSymbol(Name, Kind, Length, (TypeSymbol)ArrayType?.Clone(), (TypeSymbol)ReturnType?.Clone(), parametertypes, fields, options); ;
        }

        public static string MakeFunctionName(string name, ICollection<TypeSymbol> parameters)
        {
            if (parameters.Count > 0)
                name += $"`({parameters.Select(ts => ts.Name).Aggregate((a, b) => $"{a},{b}")})";
            else
                name += "`(void)";
            return name;
        }

        public static string MakeFunctionName(string name, ICollection<Expression> parameters)
        {
            if (parameters.Count > 0)
                name += $"`({parameters.Select(e => e.Type.Name).Aggregate((a, b) => $"{a},{b}")})";
            else
                name += "`(void)";
            return name;
        }
    }
}
