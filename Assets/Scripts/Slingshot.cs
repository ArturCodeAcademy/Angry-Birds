using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private Transform _prefab;
    [SerializeField, Range(1, 10)] private float _maxLength;
    [SerializeField, Range(1, 200)] private float _impulse;

	private Transform _currentBullet;
	private LineRenderer _lineRenderer;

	private void Awake()
	{
		_lineRenderer = GetComponentInChildren<LineRenderer>();
		_lineRenderer.enabled = false;
	}

	private void OnMouseDown()
	{
		_currentBullet = Instantiate(_prefab);
		_lineRenderer.enabled = true;
	}

	private void OnMouseDrag()
	{
		GetDistance(out Vector2 mousePos, out float distance);
		_currentBullet.position = Vector2.MoveTowards(transform.position, mousePos, distance);
		float impulse = _impulse * distance / _maxLength;
		Vector2 direction = ((Vector2)transform.position - mousePos).normalized;
		DrawTrajectory(impulse, direction);
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
		_lineRenderer.enabled = false;
	}

	private void GetDistance(out Vector2 mousePos, out float distance)
	{
		mousePos = GetMousePosition();
		distance = Vector2.Distance(mousePos, transform.position);
		distance = Mathf.Min(distance, _maxLength);
	}

	private void DrawTrajectory(float impulse, Vector2 direction)
	{
		const int COUNT = 20;
		const float TIME_DELTA = 0.1f;

		_lineRenderer.positionCount = COUNT;

		Vector2 force = direction * impulse;
		Vector3[] positions = new Vector3[COUNT];
		float time = TIME_DELTA;

		for (int i = 0; i < COUNT; i++)
		{
			positions[i] = new Vector3
			(
				force.x * time,
				force.y * time - Physics2D.gravity.magnitude * time *  time / 2
			);
			positions[i] += _currentBullet.position;
			time += TIME_DELTA;
		}

		_lineRenderer.SetPositions(positions);
	}

	private Vector2 GetMousePosition()
	{
		Vector2 mouseScreen = Input.mousePosition; 
		return Camera.main.ScreenToWorldPoint(mouseScreen);
	}
}
