using System.Collections;
using UnityEngine;

namespace CompCube.UI;

public class SharedCoroutineStarter : MonoBehaviour
{
    public void Run(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}