using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Framework.Matrox
{
    public static class MatroxObjectPool
    {
        static List<IDisposable> _disposables = new List<IDisposable>();

        public static void Add(IDisposable disposable)
        {
            lock (_disposables)
                _disposables.Add(disposable);
        }

        public static void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
