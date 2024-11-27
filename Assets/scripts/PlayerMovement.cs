using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{

    public Vector2 pos;
    public Vector2 vel;
    public Vector2 checkPointPos;
    public float jumpHeight;
    public float speed;
    public int maxJumps;
    float jumps;
    bool canJump;
    bool grounded;

    public Tilemap map;
    public TilemapRenderer mapRenderer;
    public string[] mapTileNames;

    public SpriteRenderer spriteRenderer;
    public Sprite[] playerSprite;

    const float gravity = -9;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer.sprite = playerSprite[0];
        pos=transform.position;
    }
    private void FixedUpdate()
    {
        resolveTerrainCollisions();
        playerInputs();

        if (grounded)
        {
            //friction resistance
            vel.x *= Mathf.Pow(0.1f, Time.deltaTime);
            vel.y *= Mathf.Pow(0.1f, Time.deltaTime);
        }
        else {
            //air resistance
            vel.x *= Mathf.Pow(0.99f, Time.deltaTime);
            vel.y *= Mathf.Pow(0.99f, Time.deltaTime);
        }

        vel.y += gravity*Time.fixedDeltaTime;
        pos += vel * Time.fixedDeltaTime;
        transform.position = new Vector3(pos.x,pos.y,-5);

        //Debug.Log(getFirstNumberInString("asdf12", 15));
    }

    void playerInputs() 
    {
        if (grounded)
        {
            jumps = maxJumps;
            canJump = true;
        }
        else 
        {
            jumps = Mathf.Max(maxJumps - 1, jumps);
        }

        if (Input.GetKey(KeyCode.D))
        {
            vel.x += speed*Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vel.x -= speed * Time.fixedDeltaTime;
        }

        if (!Input.GetKey(KeyCode.W)) 
        {
            canJump = true;
        }
        if (Input.GetKey(KeyCode.W)&&canJump&&jumps>0)
        {
            jumps--;
            canJump = false;
            //1/2mv^2=mgh    1/2v^2=gh   sqrt(2gh)=v
            vel.y = Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpHeight));
            grounded = false;
        }

    }

    void resolveTerrainCollisions() 
    {
       Debug.Log(getTile(pos));
        grounded = false;
        if (pos.y < 0)
        {
            pos.y = 0;
            vel.y = 0;
            grounded = true;
        }
    }


    int getTile(Vector2 inputPos)
    {
        int output = 0;
        Vector3Int gridPosition = map.WorldToCell(new Vector3(inputPos.x,inputPos.y,0));
        TileBase tile= map.GetTile(gridPosition);
        Debug.Log(tile.ToString());
        //output=int.Parse(tile.ToString());

        

        return getFirstNumberInString(tile.ToString(),100);
    }


    int getFirstNumberInString(string inputString, int maxInt)
    { 
        for(int i=maxInt;i>=0;i--)
        {

            if(inputString.IndexOf(""+i)>=0)
            {
                return i;
            }
        }
        return 0;
    }

}
