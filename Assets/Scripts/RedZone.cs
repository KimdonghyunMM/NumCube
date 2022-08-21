using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RedZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var cube = other.GetComponent<CubeController>();
        if (cube != null)
        {
            if (!cube.isMainCube && cube._cubeRb.velocity.magnitude < 0.1f)
            {
                GameManager.instance.isGameOver.Value = true;
                GameManager.instance.GameOverEvent(true);
            }
        }
    }
}
