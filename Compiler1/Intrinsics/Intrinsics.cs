using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Compiler1
{
    internal static class Intrinsics
    {
        internal static Dictionary<string, Intrinsic> LIntrinsics;
        static Dictionary<long, object> NativeHandles;

        static Intrinsics()
        {
            LIntrinsics = new Dictionary<string, Intrinsic>();
            NativeHandles = new Dictionary<long, object>();

            LIntrinsics.Add("print`(int)", new Intrinsic("print`(int)",
                TypeSymbol.FUNCTION_SYMBOL("print", "(int)->void", TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL }),
                par =>
                {
                    Console.WriteLine(par.First().GetValue());
                    return LNull.NULL;
                }));

            LIntrinsics.Add("print`(int,int)", new Intrinsic("print`(int,int)",
                TypeSymbol.FUNCTION_SYMBOL("print", "(int,int)->void", TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL, TypeSymbol.INT_SYMBOL }),
                par =>
                {
                    Console.WriteLine(Convert.ToString((long)par.ElementAt(0).GetValue(), (int)par.ElementAt(1).GetValue()));

                    return LNull.NULL;
                }));

            LIntrinsics.Add("print`(float)", new Intrinsic("print`(float)", 
                TypeSymbol.FUNCTION_SYMBOL("print", "(float)->void", TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.FLOAT_SYMBOL }),
                par =>
                {
                    Console.WriteLine(par.First().GetValue());
                    return LNull.NULL;
                }));

            LIntrinsics.Add("print`(bool)", new Intrinsic("print`(bool)", 
                TypeSymbol.FUNCTION_SYMBOL("print", "(bool)->void", TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.BOOL_SYMBOL }),
                par =>
                {
                    Console.WriteLine(par.First().GetValue());
                    return LNull.NULL;
                }));

            LIntrinsics.Add("print`(string)", new Intrinsic("print`(string)", 
                TypeSymbol.FUNCTION_SYMBOL("print", "(string)->void", TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.STRING_SYMBOL }),
                par =>
                {
                    Console.WriteLine(new string(par.First().GetValue()._str));
                    return LNull.NULL;
                }));

            LIntrinsics.Add("readInt`(void)", new Intrinsic("readInt`(void)", 
                TypeSymbol.FUNCTION_SYMBOL("readInt", "(void)->int", TypeSymbol.INT_SYMBOL, new List<TypeSymbol>() { }),
                par =>
                {
                    return new LInt(long.Parse(Console.ReadLine()));
                }));

            LIntrinsics.Add("readFloat`(void)", new Intrinsic("readFloat`(void)", 
                TypeSymbol.FUNCTION_SYMBOL("readFloat", "(void)->float", TypeSymbol.FLOAT_SYMBOL, new List<TypeSymbol>() { }),
                par =>
                {
                    return new LFloat(double.Parse(Console.ReadLine()));
                }));

            LIntrinsics.Add("readString`(void)", new Intrinsic("readString`(void)", 
                TypeSymbol.FUNCTION_SYMBOL("readString", "(void)->string", TypeSymbol.STRING_SYMBOL, new List<TypeSymbol>() { }),
                par =>
                {
                    return new LString(Console.ReadLine());
                }));

            LIntrinsics.Add("openFile`(string,int,int)", new Intrinsic("openFile`(string,int,int)", 
                TypeSymbol.FUNCTION_SYMBOL("openFile", "(string,int,int)->int", TypeSymbol.INT_SYMBOL, new List<TypeSymbol>() { TypeSymbol.STRING_SYMBOL, TypeSymbol.INT_SYMBOL, TypeSymbol.INT_SYMBOL }),
                par =>
                {
                    string fname = new string(par.ElementAt(0).GetValue()._str);
                    var fmode = (FileMode)par.ElementAt(1).GetValue();
                    var faccess = (FileAccess)par.ElementAt(2).GetValue();

                    var fs = File.Open(fname, fmode, faccess);

                    long handle = fs.SafeFileHandle.DangerousGetHandle().ToInt64();
                    NativeHandles.Add(handle, fs);

                    return new LInt(handle);
                }));

            LIntrinsics.Add("readFromFile`(int,int,int)", new Intrinsic("readFromFile`(int,int,int)", 
                TypeSymbol.FUNCTION_SYMBOL("readFromFile", "(int,int,int)->[int]", TypeSymbol.ARRAY_SYMBOL(TypeSymbol.INT_SYMBOL), new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL, TypeSymbol.INT_SYMBOL, TypeSymbol.INT_SYMBOL }),
                par =>
                {
                    long fhandle = par.ElementAt(0).GetValue();
                    var readOffset = par.ElementAt(1).GetValue();
                    var readLength = par.ElementAt(2).GetValue();


                    var fs = (FileStream)NativeHandles[fhandle];
                    var bytes = new byte[readLength];
                    fs.Read(bytes, readOffset, readLength);

                    var lInts = new LData[readLength];
                    for (int i = 0; i < readLength; i++)
                    {
                        lInts[i] = new LInt(bytes[i]);
                    }

                    return new LArray(lInts);
                }));

            LIntrinsics.Add("writeToFile`(int,[int])", new Intrinsic("writeToFile`(int,[int])", 
                TypeSymbol.FUNCTION_SYMBOL("writeToFile", "(int,[int])->int", TypeSymbol.INT_SYMBOL, new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL, TypeSymbol.ARRAY_SYMBOL(TypeSymbol.INT_SYMBOL) }),
                par =>
                {
                    long fhandle = par.ElementAt(0).GetValue();
                    var bytesToWrite = par.ElementAt(1).GetValue();


                    var fs = (FileStream)NativeHandles[fhandle];
                    foreach (var b in bytesToWrite._arr)
                    {
                        fs.WriteByte((byte)b);
                    }

                    return new LInt(bytesToWrite.length);
                }));

            LIntrinsics.Add("writeToFile`(int,string)", new Intrinsic("writeToFile`(int,string)", 
                TypeSymbol.FUNCTION_SYMBOL("writeToFile", "(int,string)->int", TypeSymbol.INT_SYMBOL, new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL, TypeSymbol.STRING_SYMBOL }),
                par =>
                {
                    long fhandle = par.ElementAt(0).GetValue();
                    var stringToWrite = par.ElementAt(1).GetValue();

                    var fs = (FileStream)NativeHandles[fhandle];
                    foreach (var b in stringToWrite._str)
                    {
                        fs.WriteByte((byte)b);
                    }

                    return new LInt(stringToWrite.length);
                }));

            LIntrinsics.Add("closeFile`(int)", new Intrinsic("closeFile`(int)", TypeSymbol.FUNCTION_SYMBOL("closeFile", "(int)->void", 
                TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL }),
                par =>
                {
                    long fhandle = par.ElementAt(0).GetValue();

                    var fs = (FileStream)NativeHandles[fhandle];
                    fs.Flush();
                    fs.Close();

                    return LNull.NULL;
                }));

            LIntrinsics.Add("exit`(int)", new Intrinsic("exit`(int)", 
                TypeSymbol.FUNCTION_SYMBOL("exit", "(int)->void", TypeSymbol.VOID_SYMBOL, new List<TypeSymbol>() { TypeSymbol.INT_SYMBOL }),
                par =>
                {
                    throw new Interpreter.ExecutionTerminatedException(par.First().GetValue());
                }));

            LIntrinsics.Add("time`(void)", new Intrinsic("time`(void)", 
                TypeSymbol.FUNCTION_SYMBOL("time", "(void)->int", TypeSymbol.INT_SYMBOL, new List<TypeSymbol>() { }),
                par =>
                {
                    var origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    return new LFloat(DateTime.Now.ToUniversalTime().Subtract(origin).TotalSeconds);
                }));
        }

        internal class Intrinsic
        {
            internal string FunctionIden;
            internal TypeSymbol FunctionSymbol;
            internal Func<ICollection<LData>, LData> InterpImplementation;

            internal Intrinsic(string iden, TypeSymbol sym, Func<ICollection<LData>, LData> interpimpl)
            {
                FunctionIden = iden;
                FunctionSymbol = sym;
                InterpImplementation = interpimpl;
            }
        }
    }
}
