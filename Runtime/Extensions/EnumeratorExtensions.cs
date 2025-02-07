using System.Collections.Generic;

namespace Mimizh.UnityUtilities
{
    public static class EnumeratorExtensions
    {
        public static IEnumerable<T> GetEnumerator<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
        
    }
}