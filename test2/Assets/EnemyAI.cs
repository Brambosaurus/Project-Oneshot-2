using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackCooldown = 2f;
    public float tacticalPause = 1f; // Pauze als vijand speler ziet

    private Transform player;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private bool isTacticallyMoving = true;

    void Start()
    {
        GameObject found = GameObject.FindGameObjectWithTag("Player");
        if (found != null)
        {
            player = found.transform;
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isTacticallyMoving)
        {
            agent.SetDestination(player.position);
        }

        if (distance <= attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
            StartCoroutine(TacticalReposition());
        }
    }

    void AttackPlayer()
    {
        Debug.Log("<color=red>Enemy attacked player!</color>");

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found on player.");
        }
    }

    System.Collections.IEnumerator TacticalReposition()
    {
        isTacticallyMoving = false;
        agent.ResetPath();

        yield return new WaitForSeconds(tacticalPause);

        // Bereken nieuwe positie naast de speler
        Vector3 offset = (transform.right + transform.forward).normalized * 2f;
        Vector3 newPosition = player.position + offset;

        agent.SetDestination(newPosition);

        yield return new WaitForSeconds(1.5f);
        isTacticallyMoving = true;
    }
}
