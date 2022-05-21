using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.ChangeCoinCount(coinValue);
        }

        Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}