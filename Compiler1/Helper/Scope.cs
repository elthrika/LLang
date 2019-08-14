using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler1
{
    public class Scope<T>// : IDictionary<string, T>
    {

        protected Scope<T> Parent = null;
        protected List<Scope<T>> Children = null;

        protected Dictionary<string, T> Values = new Dictionary<string, T>();

        //public ICollection<string> Keys => values.Keys;

        //public ICollection<T> Values => values.Values;

        //public int Count => values.Count;

        //public bool IsReadOnly => false;

        public T this[string key] { get => IsInScope(key); set => PutInScope(key, value); }

        public Scope<T> GoUp()
        {
            return Parent;
        }

        public Scope<T> GoDown()
        {
            Scope<T> ns = new Scope<T>(this);
            Children.Add(ns);
            return ns;
        }

        public Scope() : this(null)
        {
            //this.name = name;
        }

        public Scope(Scope<T> parent)
        {
            this.Parent = parent;
            Children = new List<Scope<T>>();
        }

        public T IsInScope(string key)
        {
            if (key == null) return default(T);

            if (Values.ContainsKey(key))
            {
                return Values[key];
            }
            return Parent == null ? default(T) : Parent.IsInScope(key);
        }

        public ICollection<T> GetPredicateMatches(Func<string, T, bool> predicate)
        {
            List<T> all = new List<T>();
            foreach (var key in Values.Keys)
            {
                if (predicate(key, Values[key]))
                    all.Add(Values[key]);
            }
            if(Parent != null)
                all.AddRange(Parent.GetPredicateMatches(predicate));
            return all;
        }


        public bool PutInScope(string key, T value)
        {
            if (Values.ContainsKey(key) || value == null)
            {
                return false;
            }
            Values[key] = value;
            return true;
        }

        public bool Remove(string key)
        {
            return Values.Remove(key);
        }

        public bool PutAllInScope(Dictionary<string, T> dict)
        {
            return PutAllInScope(dict.Keys, dict.Values);
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (var key in Values.Keys)
            {
                sb.AppendLine($"\t{key}={Values[key].ToString()}");
            }
            foreach (var child in Children)
            {
                string c = child.ToString();
                foreach (var line in c.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    sb.AppendLine($"\t{line}");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        //public bool ContainsKey(string key)
        //{
        //    return values.ContainsKey(key);
        //}

        //public void Add(string key, T value)
        //{
        //    PutInScope(key, value);
        //}

        //public bool Remove(string key)
        //{
        //    return values.Remove(key);
        //}

        //public bool TryGetValue(string key, out T value)
        //{
        //    return values.TryGetValue(key, out value);
        //}

        //public void Add(KeyValuePair<string, T> item)
        //{
        //    PutInScope(item.Key, item.Value);
        //}

        //public void Clear()
        //{
        //    values.Clear();
        //}

        //public bool Contains(KeyValuePair<string, T> item)
        //{
        //    return values.Contains(item);
        //}

        //public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        //{
        //    ;
        //}

        //public bool Remove(KeyValuePair<string, T> item)
        //{
        //    return values.Remove(item.Key);
        //}

        //public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        //{
        //    return values.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}
    }
}
