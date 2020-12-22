using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public static class MatroxObjectPool
    {
        static Dictionary<int, List<IDisposable>> _disposablesDictionary = new Dictionary<int, List<IDisposable>>();
        
        public static void Add(IDisposable disposable, int layer = 0)
        {
            lock (_disposablesDictionary)
            {
                if (_disposablesDictionary.ContainsKey(layer) == false)
                    _disposablesDictionary[layer] = new List<IDisposable>();

                _disposablesDictionary[layer].Add(disposable);
            }
        }

        public static void Dispose()
        {
            foreach (var pair in _disposablesDictionary.OrderBy(p => p.Key))
                foreach (var disposable in pair.Value)
                    disposable.Dispose();
        }
    }
}
