using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float detectionRange = 10f;
    public float retreatDistance = 5f;
    public float stoppingDistance = 8f;
    public float moveSpeed = 3.5f;
    public float attackCooldown = 2f;

    private float attackTimer = 0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Zoek speler automatisch op tag indien niet toegewezen
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
            else
                Debug.LogWarning("Player not found! Make sure the player has the tag 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Richting naar speler (zonder omhoog/omlaag te kijken)
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f; // Geen kanteling
        float distance = toPlayer.magnitude;
        Vector3 direction = toPlayer.normalized;

        attackTimer -= Time.deltaTime;

        // Draai naar speler
        transform.forward = direction;

        // Gedrag op basis van afstand
        if (distance < retreatDistance)
        {
            // Te dichtbij → achteruit bewegen
            controller.Move(-direction * moveSpeed * Time.deltaTime);
        }
        else if (distance <= stoppingDistance)
        {
            // Binnen schietafstand → blijven staan en schieten
            if (attackTimer <= 0f)
            {
                Shoot();
                attackTimer = attackCooldown;
            }
        }
        else if (distance < detectionRange)
        {
            // Te ver weg → vooruit bewegen
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }
        // Buiten detection range → niets doen
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = proj.GetComponent<Projectile>();
            if (projectile != null)
            {
                Vector3 shootDirection = (player.position - firePoint.position).normalized;
                projectile.SetDirection(shootDirection);
            }
        }
    }
}
