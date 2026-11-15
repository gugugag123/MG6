using UnityEngine;

public class PantryRatLifetime : MonoBehaviour
{
    private float _age;
    private void Update()
    {
        _age += Time.deltaTime;
        if (_age > 30.0f || transform.position.y < -10.0f)
            Destroy(gameObject);
    }
}
