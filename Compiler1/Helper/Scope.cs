using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    public class Scope<T>
    {

        Scope<T> parent = null;
        List<Scope<T>> children = null;

        Dictionary<string, T> values = new Dictionary<string, T>();

        public Scope<T> GoUp()
        {
            return parent;
        }

        public Scope<T> GoDown()
        {
            Scope<T> ns = new Scope<T>(this);
            children.Add(ns);
            return ns;
        }

        public Scope() : this(null)
        {
            //this.name = name;
        }

        public Scope(Scope<T> parent)
        {
            this.parent = parent;
            children = new List<Scope<T>>();
        }

        public T IsInScope(string key)
        {
            if (key == null) return default(T);

            if (values.ContainsKey(key))
            {
                return values[key];
            }
            return parent == null ? default(T) : parent.IsInScope(key);
        }

        public bool PutInScope(string key, T value)
        {
            if (values.ContainsKey(key) || value == null)
            {
                return false;
            }
            values[key] = value;
            return true;
        }

        public bool PutAllInScope(ICollection<string> keys, ICollection<T> values)
        {
            if (keys.Count != values.Count)
                throw new ArgumentException("keys and values do not have the same size");
            bool rval = true;
            for(int i = 0; i < keys.Count; i++)
            {
                rval &= PutInScope(keys.ElementAt(i), values.ElementAt(i));
            }
            return rval;
        }

    }
}
