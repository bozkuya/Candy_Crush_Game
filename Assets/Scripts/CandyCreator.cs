using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CandyCreator : MonoBehaviour
{
    public GameObject[] candy;
    public int width, height;
    public float dropSpeed = 20.0f;
    private GameObject[,] candyBoard;  // yeni 2D dizi

    private GameObject firstSelectedCandy = null;
    public void DestroyMatches()
    {
        List<GameObject> candiesToDestroy = new List<GameObject>();

        // Check horizontal matches
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                if (candyBoard[x, y] != null && candyBoard[x + 1, y] != null && candyBoard[x + 2, y] != null)
                {
                    if (candyBoard[x, y].tag == candyBoard[x + 1, y].tag && candyBoard[x, y].tag == candyBoard[x + 2, y].tag)
                    {
                        candiesToDestroy.Add(candyBoard[x, y]);
                        candiesToDestroy.Add(candyBoard[x + 1, y]);
                        candiesToDestroy.Add(candyBoard[x + 2, y]);
                    }
                }
            }
        }

        // Check vertical matches
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                if (candyBoard[x, y] != null && candyBoard[x, y + 1] != null && candyBoard[x, y + 2] != null)
                {
                    if (candyBoard[x, y].tag == candyBoard[x, y + 1].tag && candyBoard[x, y].tag == candyBoard[x, y + 2].tag)
                    {
                        candiesToDestroy.Add(candyBoard[x, y]);
                        candiesToDestroy.Add(candyBoard[x, y + 1]);
                        candiesToDestroy.Add(candyBoard[x, y + 2]);
                    }
                }
            }
        }

        // Destroy matched candies
        foreach (GameObject candyObject in candiesToDestroy.Distinct())
        {
            Destroy(candyObject);
        }
    }

    void Start()
    {
        candyBoard = new GameObject[width, height];  // diziye boyut atamasý
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newCandy = CreateCandy(x, y);
                candyBoard[x, y] = newCandy;  // yeni þekerleri diziye kaydet
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (firstSelectedCandy == null)
                {
                    firstSelectedCandy = hit.collider.gameObject;
                }
                else
                {
                    if (CanSwap(firstSelectedCandy.transform.position, hit.collider.transform.position))
                    {
                        StartCoroutine(SwapCandies(firstSelectedCandy, hit.collider.gameObject));
                    }

                    firstSelectedCandy = null;
                }
            }
        }
        
    }

    public GameObject CreateCandy(int x, int y)
    {
        GameObject NewCandy = Instantiate(RandomCandy(), new Vector2(x, y + 5), Quaternion.identity);
        StartCoroutine(DropCandy(NewCandy, new Vector2(x, y), dropSpeed));
        return NewCandy;
    }

    public GameObject RandomCandy()
    {
        int RandomCandyNumber = Random.Range(0, candy.Length);
        return candy[RandomCandyNumber];
    }

    IEnumerator DropCandy(GameObject candyObject, Vector2 targetPosition, float speed)
    {
        Vector2 initialPosition = candyObject.transform.position;
        float journeyLength = Vector2.Distance(initialPosition, targetPosition);
        float startTime = Time.time;

        while (Vector2.Distance(candyObject.transform.position, targetPosition) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;

            candyObject.transform.position = Vector2.Lerp(initialPosition, targetPosition, fractionOfJourney);
            yield return null;
        }

        candyObject.transform.position = targetPosition;
    }

    bool CanSwap(Vector2 pos1, Vector2 pos2)
    {
        return (Mathf.Abs(pos1.x - pos2.x) == 1 && pos1.y == pos2.y) || (Mathf.Abs(pos1.y - pos2.y) == 1 && pos1.x == pos2.x);
    }

    IEnumerator SwapCandies(GameObject candy1, GameObject candy2)
    {
        int x1 = (int)candy1.transform.position.x;
        int y1 = (int)candy1.transform.position.y;
        int x2 = (int)candy2.transform.position.x;
        int y2 = (int)candy2.transform.position.y;

        // Update positions in the array
        candyBoard[x1, y1] = candy2;
        candyBoard[x2, y2] = candy1;

        Vector2 pos1 = candy1.transform.position;
        Vector2 pos2 = candy2.transform.position;

        StartCoroutine(DropCandy(candy1, pos2, dropSpeed));
        StartCoroutine(DropCandy(candy2, pos1, dropSpeed));

        // Wait for the DropCandy coroutine to finish
        yield return new WaitForSeconds(0.5f);

        // Call DestroyMatches to remove any matches
        DestroyMatches();
    }

}