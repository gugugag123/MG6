using UnityEngine;

public class Rat : MonoBehaviour
{
    [SerializeField] private float _shrinkAmount;
    [SerializeField] private int _spawnAmount = 5;
    [SerializeField] private GameObject _ratPrefab;
    [SerializeField] private SpriteRenderer _explosionPrefab;

    private Color[] _colors = new Color[3]
    {
        Color.yellow,
        Color.red,
        Color.blue
    };
    private int _colorIndex;
    public int HitCount { get; private set; }
    public Color NextColor => _colors[_colorIndex];

    // ------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // STEP 2 -------------------------------------------------------------
        // If the Rat collides with a GameObject with the "Attack" tag,
        //      call SpawnSmallRat() _spawnAmount number of times to spawn
        //      more rats. (The spawning logic is written for you.)
        //
        // Also, shrink the original Rat by _shrinkAmount amount.
        //
        // STEP 3
        // Additionally, call SpawnExplosion(), and pass in a different color
        //      every time.
        // SPECIFICALLY, go through the colors in the array named _colors
        //      IN ORDER.
        // Each time the Rat is kicked, the explosion should be a different
        //      color: first yellow, then red, then blue, then looping back
        //      around to yellow, etc.
        if (collision.collider.CompareTag("Attack"))
        {
            for (int i = 0; i < _spawnAmount; i++)
            {
                SpawnSmallRat();
            }

            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(
                Mathf.Max(0.25f, scale.x * _shrinkAmount),
                Mathf.Max(0.25f, scale.y * _shrinkAmount), 1.0f);
            SpawnExplosion(_colors[_colorIndex]);
            _colorIndex = (_colorIndex + 1) % _colors.Length;
            HitCount++;
        }
        // STEP 2 -------------------------------------------------------------
    }

    // ------------------------------------------------------------------------
    private void SpawnExplosion(Color color)
    {
        SpriteRenderer explosion = Instantiate(_explosionPrefab);
        explosion.transform.position = transform.position;
        explosion.color = color;
        explosion.gameObject.SetActive(true);
    }

    // ------------------------------------------------------------------------
    private void SpawnSmallRat()
    {
        GameObject rat = Instantiate(_ratPrefab);

        Vector2 offset = Random.insideUnitCircle * 0.5f;
        rat.transform.position = transform.position + new Vector3(offset.x, Mathf.Abs(offset.y), 0);
        rat.AddComponent<PantryRatLifetime>();
        Rigidbody2D body = rat.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = new Vector2(Random.Range(-3.0f, 3.0f), Random.Range(3.5f, 6.5f));
        }

        // STEP 3 -------------------------------------------------------------
        // Uncomment and fix the below lines to give the small rat a random
        //      rotation correctly.
        // Your game should look exaclty like the finished example.

        float randomAngle = Random.Range(0.0f, 360.0f);
        Vector3 rotationAngles = new Vector3(0, 0, randomAngle);
        rat.transform.Rotate(rotationAngles);

        // STEP 3 -------------------------------------------------------------
    }
}
