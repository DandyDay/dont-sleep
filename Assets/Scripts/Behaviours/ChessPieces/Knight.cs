using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPieceBehaviour
{

    public Animator KnightJump;
    public float acceleration;
    public override void Attack()
    {
        Vector3 direction;
        if (CalculateDistanceFromPlayer(out direction) > Mathf.Sqrt(7) * spotSize)
        {
            direction.Normalize();
            direction *= spotSize * Mathf.Sqrt(7);
        }
        StartCoroutine(JumpToPlayer(direction));
    }

    private IEnumerator JumpToPlayer(Vector3 direction)
    {
        Vector3 initialPos = transform.position;
        float time = 0;
        float movingTime = 1f;
        while (time < movingTime)
        {
            time += Time.deltaTime;
            transform.position = GetPosition(time, initialPos, direction, movingTime);
            yield return null;
        }
        transform.position = initialPos + direction;
    }

    private IEnumerator AttackCoroutine()
    {
        Vector3 playerPos = GameManager.GetInstance().player.transform.position; // world position
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        Attack();
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    public override void Activate(bool promoted, int row)
    {
        base.Activate(promoted, row);
        maxHealth = 10;
        health = maxHealth;
        damage = 10;
        acceleration = 0.1f;
        StartCoroutine(AttackCoroutine());
    }

    private void OnDestroy()
    {
        if (health == 0)
        {

            if (promoted) DeadPawnCount++;
            DeadPieceCount++;
        }
    }

    public override void Update()
    {
        if (DeadPawnCount == 8 && !activated) Activate(false, 0);
        base.Update();
    }


    private Vector3 GetPosition(float time, Vector3 initial, Vector3 direction, float movingTime)
    {
        Vector3 position = initial + time * direction / movingTime + Vector3.up * -acceleration * time * (time - movingTime);
        return position;
    }
}
