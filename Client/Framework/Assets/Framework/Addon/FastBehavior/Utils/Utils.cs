using UnityEngine;
using System.Collections.Generic;

namespace FastBehavior
{
    /// <summary>
    /// <para>Class Introduce</para>
    /// <para>Author: zhengnan</para>
    /// <para>Create: 2019/5/28 0:27:27</para>
    /// </summary> 
    /// 
    public static class Utils
    {
        public static int[] GetRandomArray(int num)
        {
            int[] list = new int[num];
            for (int i = 0; i < list.Length; i++)
                list[i] = i;
            for (int i = 0; i < list.Length; i++)
            {
                int r = Random.Range(i, list.Length);
                int tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
            return list;
        }
    }
}

