using UnityEngine;

public class TutorialEndTrigger : MonoBehaviour
{
    private bool done;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (done || !other.CompareTag("Player")) return;
        done = true;
        GameEvents.RaiseTutorialCompleted();
    }
}