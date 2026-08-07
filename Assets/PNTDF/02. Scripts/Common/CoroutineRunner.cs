using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class CoroutineRunner : LocalSingleton<CoroutineRunner>
    {
        public Coroutine Run(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }
}



