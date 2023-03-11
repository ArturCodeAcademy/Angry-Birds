using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private Transform _prefab;
    [SerializeField, Range(1, 10)] private float _maxLength;
    [SerializeField, Range(1, 200)] private float _impulse;

	private Transform _currentBullet;

	private void OnMouseDown()
	{
		_currentBullet = Instantiate(_prefab);
	}

	private void OnMouseDrag()
	{
		GetDistance(out Vector2 mousePos, out float distance);
		distance = Mathf.Min(distance, _maxLength);
		_currentBullet.position = Vector2.MoveTowards(transform.position, mousePos, distance);
	}

	private void OnMouseUp()
	{
		Vector2 direction = (Vector2)(transform.position - _currentBullet.position).normalized;
		GetDistance(out Vector2 _, out float distance);
		float impulse = _impulse * distance / _maxLength;
		Rigidbody2D rb = _currentBullet.gameObject.AddComponent<Rigidbody2D>();
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.AddForce(direction * impulse, ForceMode2D.Impulse);
		_currentBullet = null;
	}

	private void GetDistance(out Vector2 mousePos, out float distance)
	{
		mousePos = GetMousePosition();
		distance = Vector2.Distance(mousePos, transform.position);
	}

	private Vector2 GetMousePosition()
	{
		Vector2 mouseScreen = Input.mousePosition; 
		return Camera.main.ScreenToWorldPoint(mouseScreen);
	}
}
