using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(3);
        while (transform.localScale.x > 0.1f)
        {
            transform.localScale /= 1.1f;
            yield return null;
        }
        gameObject.SetActive(false);
        yield return null;
    }

}
