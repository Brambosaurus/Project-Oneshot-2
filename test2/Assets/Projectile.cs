using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    public float speed = 10f;

    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Als we de speler raken
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        // Als we iets anders raken dat géén Enemy is (bijv. muur, grond…)
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
