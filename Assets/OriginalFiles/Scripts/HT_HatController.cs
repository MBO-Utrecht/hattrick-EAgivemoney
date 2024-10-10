using UnityEngine;
using System.Collections;

public class HT_HatController : MonoBehaviour
{
    public Camera cam;
    public float moveSpeed = 8.0f;
    public float bombAvoidanceRadius = 6.0f;
    public float bombAvoidanceStrength = 8.0f;

    private float maxWidth;
    private bool canControl = true;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0.0f);
        Vector3 targetWidth = cam.ScreenToWorldPoint(upperCorner);
        float hatWidth = GetComponent<Renderer>().bounds.extents.x;
        maxWidth = targetWidth.x - hatWidth;
    }

    void FixedUpdate()
    {
        if (canControl)
        {
            GameObject nearestBomb = FindClosestBomb();
            GameObject targetBall = FindClosestBall();

            if (nearestBomb != null && Vector3.Distance(transform.position, nearestBomb.transform.position) < bombAvoidanceRadius)
            {
                Vector3 avoidDirection = (transform.position - nearestBomb.transform.position).normalized;
                Vector3 targetPosition = transform.position + avoidDirection * bombAvoidanceStrength;

                MoveTowards(new Vector3(targetPosition.x, transform.position.y, transform.position.z));
            }
            else if (targetBall != null)
            {
                MoveTowards(new Vector3(targetBall.transform.position.x, transform.position.y, transform.position.z));
            }
        }
    }

    GameObject FindClosestBall()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        GameObject closestBall = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject ball in balls)
        {
            float distance = Vector3.Distance(ball.transform.position, currentPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBall = ball;
            }
        }

        return closestBall;
    }

    GameObject FindClosestBomb()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        GameObject closestBomb = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject bomb in bombs)
        {
            float distance = Vector3.Distance(bomb.transform.position, currentPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBomb = bomb;
            }
        }

        return closestBomb;
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 direction = (targetPosition - currentPosition).normalized;
        Vector3 newPosition = currentPosition + direction * moveSpeed * Time.fixedDeltaTime;

        float clampedX = Mathf.Clamp(newPosition.x, -maxWidth, maxWidth);
        newPosition = new Vector3(clampedX, transform.position.y, transform.position.z);

        GetComponent<Rigidbody2D>().MovePosition(newPosition);
    }

    public void ToggleControl(bool toggle)
    {
        canControl = toggle;
    }
}