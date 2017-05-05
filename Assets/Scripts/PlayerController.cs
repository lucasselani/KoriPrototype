using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveTime = 5f;
	public LayerMask BlockingLayer;
	public GameObject Projectile;

    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
	private BoxCollider2D boxCollider;
	private Animator animator;
	private float inverseMoveTime;
	private float projectileSpeed = 3f;
	private float fireCooldown = 2f;
	private float attackCooldown = 0.5f;
	private float nextFire = 0;
	private float nextAttack = 0;

	// Use this for initialization
	void Start () {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
		boxCollider = GetComponent<BoxCollider2D>();
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		inverseMoveTime = 1f/moveTime;
			
	}

	private bool Move(int xDir, int yDir, out RaycastHit2D hit){ 
		 Vector2 start = this.transform.position;
         Vector2 end = start +  new Vector2(xDir,yDir) * this.inverseMoveTime * Time.deltaTime;

		 boxCollider.enabled = false;
         hit = Physics2D.Linecast (start, end, BlockingLayer);
		 boxCollider.enabled = true;
 
         if(hit.transform == null){		
			 StartCoroutine(SmoothMovement(end));	 
			 return true;
		 } else	return false;		 
	}

	protected IEnumerator SmoothMovement(Vector3 end){
		Vector3 newPosition = new Vector3(end.x, end.y, 0f);
        rb2d.MovePosition (newPosition);
		yield return null;
	}

	private void AttemptMove(){
		int horizontal = (int) Input.GetAxisRaw("Horizontal");
		int vertical = (int) Input.GetAxisRaw("Vertical");

		if(horizontal != 0 || vertical != 0){
			animator.SetTrigger("walk");
		} else{
			animator.SetTrigger("idle");
			return;
		} 
		RaycastHit2D hit;
		bool canMove = Move(horizontal, vertical, out hit);
		if(!canMove) CheckCollision(hit);
	}

	private void CheckCollision(RaycastHit2D hit){
		if(hit.collider.tag == "Block") Debug.Log("Block");
	}

	void Attack(){
		if (Input.GetButton("Fire2") && Time.time > nextFire){
			nextFire = Time.time + fireCooldown;

			Vector3 mouse = Input.mousePosition;
			mouse.z = 0.0f;
			mouse = Camera.main.ScreenToWorldPoint(mouse);
			mouse = mouse-transform.position;

			GameObject projectile = Instantiate(Projectile, transform.position, Quaternion.identity);
			projectile.GetComponent<Rigidbody2D>().velocity = (new Vector3(mouse.x*projectileSpeed,mouse.y*projectileSpeed,0f));
			Destroy(projectile.gameObject,3);
		} else if(Input.GetButton("Fire1") && Time.time > nextAttack){
			nextAttack = Time.time + attackCooldown;

			Vector3 mouse = Input.mousePosition;
			mouse.z = 0.0f;
			mouse = Camera.main.ScreenToWorldPoint(mouse);
			mouse = mouse-transform.position;	
			StartCoroutine(AttackSmooth());		
		}
	}

	IEnumerator AttackSmooth(){
		animator.SetTrigger("attack");
		yield return new WaitForSeconds(2f);
	}

	
	// Update is called once per frame
	void FixedUpdate () {	
		AttemptMove();
		Attack();
	}
}
