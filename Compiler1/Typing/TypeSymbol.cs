using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    public class TypeSymbol : ICloneable
    {
        public enum TypeKind
        {
            PRIMITIVE,
            ARRAY,
            STRUCT,
            FUNCTION,
            POINTER,
            INFER
        }

        public static readonly TypeSymbol INT_SYMBOL = new TypeSymbol("int", TypeKind.PRIMITIVE, 8); // 8 byte size integer
        public static readonly TypeSymbol FLOAT_SYMBOL = new TypeSymbol("float", TypeKind.PRIMITIVE, 8); // 8 byte size floatingpoint number
        public static readonly TypeSymbol VOID_SYMBOL = new TypeSymbol("void", TypeKind.PRIMITIVE, 0); // 0 size void type
        public static readonly TypeSymbol BOOL_SYMBOL = new TypeSymbol("bool", TypeKind.PRIMITIVE, 1); // 1 byte bool
        public static readonly TypeSymbol STRING_SYMBOL = new TypeSymbol("string", TypeKind.STRUCT, 16, new Dictionary<string, TypeSymbol>(2) { { "length", INT_SYMBOL }, { "_str", POINTER_SYMBOL(INT_SYMBOL) } }, new List<int>() { 0, 8 }); // { int length, char* str }

        public static TypeSymbol POINTER_SYMBOL(TypeSymbol type)
            => new TypeSymbol($"ptr::{type.Name}", TypeKind.POINTER, 8, type.ArrayType);

        public static TypeSymbol INFER_SYMOBOL(string name) 
            => new TypeSymbol(name, TypeKind.INFER, 0); // Type to be determined, name=typename in source

        public static TypeSymbol ARRAY_SYMBOL(TypeSymbol arraytype) 
            => new TypeSymbol("["+arraytype.Name+"]", TypeKind.ARRAY, 16, arraytype, null, null, new Dictionary<string, TypeSymbol>(2) { { "length", INT_SYMBOL }, { "_arr", POINTER_SYMBOL(arraytype) } }, new List<int>() { 0, 8 }); // { int length, T* array }

        public static TypeSymbol FUNCTION_SYMBOL(string name, TypeSymbol rettype, List<TypeSymbol> paramtypes) 
            => new TypeSymbol(name, TypeKind.FUNCTION, rettype, paramtypes);

        public static TypeSymbol STRUCT_SYMBOL(string name, int length, Dictionary<string, TypeSymbol> fields, List<int> offsets)
            => new TypeSymbol(name, TypeKind.STRUCT, length, fields, offsets);

        //@TODO: Maybe make this a Scope, so that types can exist on certain levels, and not only globally
        public readonly string Name;
        //public readonly string FunctionName;
        public readonly TypeKind Kind;
        public readonly int Length;
        public readonly TypeSymbol ArrayType;
        public readonly TypeSymbol ReturnType;
        public readonly List<TypeSymbol> ParameterTypes;
        public readonly Dictionary<string, TypeSymbol> Fields;
        public readonly List<int> Offsets;

        public TypeSymbol(string name, TypeKind kind, int length, TypeSymbol arrayof, TypeSymbol rettype, List<TypeSymbol> parameters, Dictionary<string, TypeSymbol> fields, List<int> offsets)
        {
            Name = name;
            Kind = kind;
            ArrayType = arrayof;
            ReturnType = rettype;
            ParameterTypes = parameters;
            Fields = fields;
            Offsets = offsets;
            Length = length;
        }

        public TypeSymbol(string name, TypeKind kind, int len) : this(name, kind, len, null, null, null, null, null) { }
        public TypeSymbol(string name, TypeKind kind, int len, TypeSymbol arrayof) : this(name, kind, len, arrayof, null, null, null, null) { }
        public TypeSymbol(string name, TypeKind kind, TypeSymbol rettype, List<TypeSymbol> parameters) : this(name, kind, 8, null, rettype, parameters, null, null) { }
        public TypeSymbol(string name, TypeKind kind, int len, Dictionary<string, TypeSymbol> fields, List<int> offsets) : this(name, kind, len, null, null, null, fields, offsets) { }

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
            Dictionary<string, TypeSymbol> fields = null;
            List<int> offsets = null;

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
                fields = new Dictionary<string, TypeSymbol>(Fields.Count);
                foreach (KeyValuePair<string, TypeSymbol> s in Fields)
                {
                    fields.Add((string)s.Key.Clone(), (TypeSymbol)s.Value.Clone());
                }
            }
            if (Offsets != null)
            {
                offsets = new List<int>(Offsets.Count);
                foreach (int i in Offsets)
                {
                    offsets.Add(i);
                }
            }

            return new TypeSymbol(Name, Kind, Length, (TypeSymbol)ArrayType?.Clone(), (TypeSymbol)ReturnType?.Clone(), parametertypes, fields, offsets); ;
        }
    }
}
