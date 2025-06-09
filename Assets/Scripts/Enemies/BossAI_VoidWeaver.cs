using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BossHealth))]
public class BossAI_VoidWeaver : MonoBehaviour
{
    [Header("Referencias")]
    public Transform playerTarget;
    private BossHealth bossHealth;
    private Rigidbody2D rb;
    public Transform[] teleportPoints; // Puntos fijos en la sala a los que se puede teletransportar

    [Header("Configuración de Ataque")]
    public GameObject shadowProjectilePrefab;
    public Transform projectileSpawnPoint;
    public int projectilesPerVolley = 5;
    public float timeBetweenProjectiles = 0.2f;
    public GameObject shadowlingMinionPrefab;
    public Transform[] minionSpawnPoints;
    public int minionsToSpawn = 2;
    public float attackCycleCooldown = 3f;

    private bool isActing = false; // Para evitar solapar acciones

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossHealth = GetComponent<BossHealth>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;
        StartCoroutine(AttackPatternCoroutine());
    }

    IEnumerator AttackPatternCoroutine()
    {
        while (bossHealth.IsAlive())
        {
            if (isActing)
            {
                yield return null;
                continue;
            }

            isActing = true;

            // Teletransportarse a un nuevo punto
            yield return StartCoroutine(Teleport());

            // Decidir qué ataque hacer
            if (bossHealth.currentPhase == BossHealth.BossPhase.Phase1)
            {
                yield return StartCoroutine(ShootVolley());
            }
            else if (bossHealth.currentPhase == BossHealth.BossPhase.Phase2)
            {
                // En fase 2, puede disparar o invocar
                if (Random.value < 0.6f) // 60% de probabilidad de disparar
                {
                    yield return StartCoroutine(ShootVolley());
                }
                else // 40% de probabilidad de invocar
                {
                    yield return StartCoroutine(SummonMinions());
                }
            }

            yield return new WaitForSeconds(attackCycleCooldown);
            isActing = false;
        }
    }

    IEnumerator Teleport()
    {
        Debug.Log("Jefe: Teletransportándose...");
        // Opcional: Activar efecto de "desaparecer"
        yield return new WaitForSeconds(0.2f); // Pequeña pausa

        if (teleportPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, teleportPoints.Length);
            transform.position = teleportPoints[randomIndex].position;
        }

        // Opcional: Activar efecto de "aparecer"
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator ShootVolley()
    {
        Debug.Log("Jefe: Lanzando ráfaga de proyectiles...");
        if (shadowProjectilePrefab == null || projectileSpawnPoint == null || playerTarget == null)
        {
            yield break; // No hacer nada si falta algo
        }

        for (int i = 0; i < projectilesPerVolley; i++)
        {
            // Calcular dirección hacia el jugador en el momento del disparo
            Vector2 directionToPlayer = (playerTarget.position - projectileSpawnPoint.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            Instantiate(shadowProjectilePrefab, projectileSpawnPoint.position, rotation);
            yield return new WaitForSeconds(timeBetweenProjectiles); // Pausa entre cada disparo
        }
    }

    IEnumerator SummonMinions()
    {
        Debug.Log("Jefe: Invocando esbirros...");
        if (shadowlingMinionPrefab == null || minionSpawnPoints.Length == 0)
        {
            yield break;
        }

        for (int i = 0; i < minionsToSpawn; i++)
        {
            int spawnIndex = Random.Range(0, minionSpawnPoints.Length);
            Instantiate(shadowlingMinionPrefab, minionSpawnPoints[spawnIndex].position, Quaternion.identity);
        }
        yield return new WaitForSeconds(1f); // Tiempo de la "animación" de invocar
    }
    
    public void OnDeath()
    {
        StopAllCoroutines();
        this.enabled = false;
    }
}