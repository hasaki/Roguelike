using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
	public float MoveTime = 0.1f;
	public LayerMask BlockingLayer;

	private BoxCollider2D _boxCollider;
	private Rigidbody2D _rigidbody;
	private float _inverseMoveTime;

	protected virtual void Start()
	{
		_boxCollider = GetComponent<BoxCollider2D>();
		_rigidbody = GetComponent<Rigidbody2D>();

		_inverseMoveTime = 1f / MoveTime;
	}

	protected IEnumerator SmoothMovement(Vector3 end)
	{
		var sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon)
		{
			var newPosition = Vector3.MoveTowards(_rigidbody.position, end, _inverseMoveTime * Time.deltaTime);
			_rigidbody.MovePosition(newPosition);

			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			yield return null;
		}
	}

	protected abstract void OnCantMove<T>(T component)
		where T : Component;

	protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;
		var end = start + new Vector2(xDir, yDir);

		_boxCollider.enabled = false;
		hit = Physics2D.Linecast(start, end, BlockingLayer);
		_boxCollider.enabled = true;

		if (hit.transform == null)
		{
			StartCoroutine(SmoothMovement(end));
			return true;
		}

		return false;
	}

	protected virtual void AttemptMove<T>(int xDir, int yDir) 
		where T : Component
	{
		RaycastHit2D hit;
		var canMove = Move(xDir, yDir, out hit);

		if (hit.transform == null)
			return;

		var hitComponent = hit.transform.GetComponent<T>();
		if (!canMove && hitComponent != null)
			OnCantMove(hitComponent);
	}
}
