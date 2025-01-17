using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
//player movement
//fix jump
//water
//particles


public class Player : MonoBehaviour
{
    public Vector2Int pos;
    public Vector2Int vel;
    public Vector2Int checkPointPos;
    public float jumpHeight;
    float normalJumpHeight;
    public float speed;
    public float groundFriction;
    public float airResistance;
    float normalSpeed;
    public int maxJumps;
    public float jumps;
    bool canJump;
    bool grounded;
    public float jumpCountDown;
    public bool jumped;
    bool inWater;

    public LevelManager levelManager;

    public SpriteRenderer spriteRenderer;
    public Sprite[] playerSprites;


    const float gravity = -30;

    public bool UsesArrowKeys;

    int AnimationFrameCount = 0;
    public float AnimationFrameRate = 8;
    float AnimationFrameCountdown = 0.25f;
    public bool facing;

    public List<Item> items = new List<Item>();

    public List<Vector2Int> checkPointHits = new List<Vector2Int>();
    [Serialize]
    public List<Vector2> itemtHits = new List<Vector2>();

    public Player otherPlayer;
    public ItemUIManager itemUIManager;

    public GameObject checkPointParticles;
    public GameObject ItemParticlesThisPlayer;
    public GameObject ItemParticlesOtherPlayer;

    public bool finished;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalSpeed = speed;
        normalJumpHeight = jumpHeight;
        spriteRenderer.sprite = playerSprites[4];
        pos = floatingToFixed((Vector2)transform.position);
        pos -= floatingToFixed(new Vector2(0.5f, 0.5f));

        //Debug.Log("Maths");
        Debug.Log(floatingToFixed(0.7173767f) + " " + floatingToFixed(0.997895f) + " " + fixedPointMult(floatingToFixed(0.7173767f), floatingToFixed(0.997895f)));
        Debug.Log(fixedToFloating(fixedPointMult(floatingToFixed(0.7173767f), floatingToFixed(0.997895f))));

    }

    private void Update()
    {

        //Debug.Log(pos.x + " " + floatingToFixed(pos.x) + " " + fixedToFloating(floatingToFixed(pos.x)));
        //Debug.Log(fixedToFloating(pos) + " " + fixedToFloating(vel));
        updateAnimations();
        itemUpdate();
    }

    private void FixedUpdate()
    {

        // Debug.Log(levelManager.getTile(transform.position));

        if (fixedToFloating(pos).y >186)
        {
            finished = true;
        }


        if (!finished)
        {


            vel.y += floatingToFixed(gravity * Time.fixedDeltaTime);
            int subSteps = 4;
            playerInputs(1);
            grounded = false;

            //Debug.Log(fixedPointMult(vel, floatingToFixed(Time.fixedDeltaTime / (float)subSteps)) + " " + (Time.fixedDeltaTime / (float)subSteps));
            //Debug.Log(fixedToFloating(fixedPointMult(vel, floatingToFixed(Time.fixedDeltaTime / (float)subSteps))) +" "+fixedToFloating(vel) +" "+" "+(Time.fixedDeltaTime / (float)subSteps));
            for (int i = 0; i < subSteps; i++)
            {
                pos += fixedPointMult(vel, floatingToFixed(Time.fixedDeltaTime / (float)subSteps));
                resolveTerrainCollisions();
            }
            if (grounded)
            {
                jumps = maxJumps;
                canJump = true;
            }
            else
            {
                if(!(jumpCountDown > 0.0f && jumped))
                {
                jumps = Mathf.Min(maxJumps - 1, jumps);
                }

            }


            transform.position = new Vector3(fixedToFloating(pos.x) + 0.5f, fixedToFloating(pos.y) + 0.5f, -5);
            if (grounded)
            {
                //friction resistance
                /*Debug.Log(fixedToFloating(vel.x)+" "+ Mathf.Pow(groundFriction, Time.deltaTime)+" "+ 
                    Mathf.Pow(groundFriction, Time.deltaTime)+" "+
                    fixedToFloating(floatingToFixed(Mathf.Pow(groundFriction, Time.deltaTime))) + " " +
                    fixedToFloating(fixedPointMult(vel.x, floatingToFixed(Mathf.Pow(groundFriction, Time.deltaTime)))));*/
                vel.x = fixedPointMult(vel.x, floatingToFixed(Mathf.Pow(groundFriction, Time.deltaTime)));

                if (!((Input.GetKey(KeyCode.D) && !UsesArrowKeys) || (Input.GetKey(KeyCode.RightArrow) && UsesArrowKeys) ||
                    (Input.GetKey(KeyCode.A) && !UsesArrowKeys) || (Input.GetKey(KeyCode.LeftArrow) && UsesArrowKeys)))
                {
                    vel.x = fixedPointMult(vel.x, floatingToFixed(Mathf.Pow(groundFriction, Time.deltaTime)));
                    vel.x = fixedPointMult(vel.x, floatingToFixed(Mathf.Pow(groundFriction, Time.deltaTime)));
                }


                // vel.y *= Mathf.Pow(0.01f, Time.deltaTime);
            }
            else
            {
                //air resistance
                vel.x = fixedPointMult(vel.x, floatingToFixed(Mathf.Pow(airResistance, Time.deltaTime)));
                // vel.y *= Mathf.Pow(0.05f, Time.deltaTime);
            }

            tileCheck();
        }
        else 
        {
            vel.y += floatingToFixed(gravity * Time.fixedDeltaTime);
            vel.y = Mathf.Max(floatingToFixed(2), vel.y);
            pos.y += fixedPointMult(vel.y, floatingToFixed(Time.fixedDeltaTime));
            transform.position = new Vector3(fixedToFloating(pos.x) + 0.5f, fixedToFloating(pos.y) + 0.5f, -5);
            if (pos.y > floatingToFixed(196))
            {
                Debug.Log("winner!");
            }
        }

    }

    void playerInputs(int subSteps)
    {

        if ((Input.GetKey(KeyCode.D) && !UsesArrowKeys) || (Input.GetKey(KeyCode.RightArrow) && UsesArrowKeys))
        {
            vel.x += floatingToFixed(speed * Time.fixedDeltaTime / (float)subSteps);
            facing = true;
        }
        if ((Input.GetKey(KeyCode.A) && !UsesArrowKeys) || (Input.GetKey(KeyCode.LeftArrow) && UsesArrowKeys))
        {
            vel.x -= floatingToFixed(speed * Time.fixedDeltaTime / (float)subSteps);
            facing = false;
        }

        if (!((Input.GetKey(KeyCode.W) && !UsesArrowKeys) || (Input.GetKey(KeyCode.UpArrow) && UsesArrowKeys)))
        {
            canJump = true;
            jumpCountDown = 0.1f;
            jumped = false;
        }

        Debug.Log(jumps);
        if (((Input.GetKey(KeyCode.W) && !UsesArrowKeys) || (Input.GetKey(KeyCode.UpArrow) && UsesArrowKeys)) && canJump && (jumps > 0||(jumpCountDown>0.0f&&jumped)) && !inWater)
        {
            jumpCountDown -= Time.deltaTime;
            if (jumpCountDown > 0.0f)
            {
                jumped = true;
                vel.y =Mathf.Max(0,vel.y+floatingToFixed(Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpHeight))*(1/0.1f)*Time.deltaTime));
            }
            else 
            {
                jumps--;
                canJump = false;
                //1/2mv^2=mgh    1/2v^2=gh   sqrt(2gh)=v
                vel.y = floatingToFixed(Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpHeight)));
                grounded = false;
            }
        }


        //swimming
        if (inWater)
        {
            if (((Input.GetKey(KeyCode.W) && !UsesArrowKeys) || (Input.GetKey(KeyCode.UpArrow) && UsesArrowKeys)))
            {
                vel.y += floatingToFixed(-gravity * jumpHeight / 4.0f * Time.deltaTime * Mathf.Max(1.0f, WaterDepth()) * 1.2f);
                float depth = WaterDepth();
                Debug.Log(depth + " " + vel.y);
                if (depth < 0.5f && vel.y > 0)
                {
                    // pos.y += floatingToFixed(0.5f);
                    vel.y += floatingToFixed(-gravity * 10.0f * Time.deltaTime);
                }
            }
            if (((Input.GetKey(KeyCode.S) && !UsesArrowKeys) || (Input.GetKey(KeyCode.DownArrow) && UsesArrowKeys)))
            {
                vel.y -= floatingToFixed(-gravity * jumpHeight / 4.0f * Time.deltaTime * WaterDepth() * 1.8f);
            }
        }


    }

    void resolveTerrainCollisions()
    {
        //Debug.Log("1 " + getTile(pos));
        ////Debug.Log("2 " + getTile(new Vector2(pos.x + 1, pos.y)));
        //Debug.Log("3 " + getTile(new Vector2(pos.x , pos.y+1)));
        //Debug.Log("4 " + getTile(new Vector2(pos.x + 1, pos.y+1)));
        //grounded = false;



        // //Debug.Log("1 " + getTile(pos));
        if (levelManager.getTile(fixedToFloating(pos)) == 1)
        {
            //   //Debug.Log("1");
            if (fixedFloor(pos.x) + 65536 - pos.x < fixedFloor(pos.y) + 65536 - pos.y)
            {
                //Debug.Log("1 x");
                pos.x = fixedFloor(pos.x) + 65536;
                vel.x = Mathf.Max(0, vel.x);
            }
            else
            {
                //Debug.Log("1 y");
                pos.y = fixedFloor(pos.y) + 65536;
                vel.y = Mathf.Max(0, vel.y);
                //grounded = true;
            }
        }
        //Debug.Log("2 " + getTile(new Vector2(pos.x + 1, pos.y)));
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(1, 0)) == 1)
        {
            //Debug.Log("2");
            if (pos.x - fixedFloor(pos.x) < fixedFloor(pos.y) + 65536 - pos.y)
            {
                //Debug.Log("2 x");
                pos.x = fixedFloor(pos.x);
                vel.x = Mathf.Min(0, vel.x);
            }
            else
            {
                //Debug.Log("2 y");
                pos.y = fixedFloor(pos.y) + 65536;
                vel.y = Mathf.Max(0, vel.y);
                //  grounded = true;
            }
        }
        //Debug.Log("3 " + getTile(new Vector2(pos.x, pos.y+1)));
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0, 1)) == 1)
        {
            //Debug.Log("3");

            if (fixedFloor(pos.x) + 65536 - pos.x < pos.y - fixedFloor(pos.y))
            {
                //Debug.Log("3 x");
                pos.x = fixedFloor(pos.x) + 65536;
                vel.x = Mathf.Max(0, vel.x);
            }
            else
            {
                //Debug.Log("3 y");
                pos.y = fixedFloor(pos.y);
                vel.y = Mathf.Min(0, vel.y);
            }
        }
        //Debug.Log("4 " + getTile(new Vector2(pos.x + 1, pos.y+1)));
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(1, 1)) == 1)
        {
            //Debug.Log("4");
            if (pos.x - fixedFloor(pos.x) < pos.y - fixedFloor(pos.y))
            {
                ////Debug.Log("4 x");
                pos.x = fixedFloor(pos.x);
                vel.x = Mathf.Min(0, vel.x);
            }
            else
            {
                //Debug.Log("4 y");
                pos.y = fixedFloor(pos.y);
                vel.y = Mathf.Min(0, vel.y);
            }
        }

        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0, -1)) == 1)
        {

            if (AABB(fixedToInt(pos) + new Vector2(0, -1), fixedToInt(pos) + new Vector2(1,0), fixedToFloating(pos) + new Vector2(0.1f, -0.1f), fixedToFloating(pos) + new Vector2(0.9f, 0.1f)))
            {
                grounded = true;
            }
        }
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(1, -1)) == 1)
        {
            if (AABB(fixedToInt(pos) + new Vector2(1, -1), fixedToInt(pos) + new Vector2(2,0), fixedToFloating(pos) + new Vector2(0.1f, -0.1f), fixedToFloating(pos) + new Vector2(0.9f, 0.1f)))
            {
                grounded = true;
            }

        }



    }

    void tileCheck()
    {
        //checkPoint check

        if (levelManager.getTile(fixedToInt(pos)) == 4) { checkPointCheck(pos); }
        if (levelManager.getTile(fixedToInt(pos) + new Vector2Int(1, 0)) == 4) { checkPointCheck(pos + intToFixed(new Vector2Int(1, 0))); }
        if (levelManager.getTile(fixedToInt(pos) + new Vector2Int(0, 1)) == 4) { checkPointCheck(pos + intToFixed(new Vector2Int(0, 1))); }
        if (levelManager.getTile(fixedToInt(pos) + new Vector2Int(1, 1)) == 4) { checkPointCheck(pos + intToFixed(new Vector2Int(1, 1))); }

        //Death check
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0.5f, 0.5f)) == 3) { Died(); }


        //water check
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0.5f, 0.5f)) == 2)
        {
            inWater = true;
            //damping
            vel.x = fixedPointMult(vel.x, floatingToFixed(Mathf.Pow(0.1f, Time.deltaTime)));
            vel.y = fixedPointMult(vel.y, floatingToFixed(Mathf.Pow(0.1f, Time.deltaTime)));

            vel.x = Mathf.Clamp(vel.x, floatingToFixed(-2.8f), floatingToFixed(2.8f));
            vel.y = Mathf.Clamp(vel.y, floatingToFixed(-8.0f), floatingToFixed(8.0f));

            //depth estimate
            float depth = WaterDepth();


            vel.y -= floatingToFixed(gravity * Mathf.Max(depth, 1.0f) * 1.3f * Time.fixedDeltaTime);
        }
        else { inWater = false; }

        //item check
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0.5f, 0.5f)) == 5)
        {
            getItem(Vector2Int.FloorToInt(fixedToFloating(pos) + new Vector2(0.5f, 0.5f)));
        }
    }

    void getItem(Vector2Int inPos)
    {
        bool hasBeen = false;

        for (int i = 0; i < itemtHits.Count; i++)
        {
            if (Vector2.Distance(itemtHits[i], inPos) < 5)
            {
                hasBeen = true;
            }
        }

        if (!hasBeen)
        {
            itemtHits.Add(inPos);
            bool itemForUs = false;
            float rand = Random.Range(0.0f, 1.0f);
            if (rand < 0.2f)
            {
                items.Add(new Item(0, 0, 15, 2, normalJumpHeight, normalSpeed));
                itemUIManager.addItem(items.Count - 1);
                Debug.Log("double jump");
                itemForUs = true;
            }
            else if (rand < 0.4f)
            {

                items.Add(new Item(0, 1, 15, 1, normalJumpHeight, normalSpeed * 1.5f));
                itemUIManager.addItem(items.Count - 1);
                Debug.Log("faster");
                itemForUs = true;
            }
            else if (rand < 0.6f)
            {
                items.Add(new Item(0, 2, 15, 1, normalJumpHeight * 1.5f, normalSpeed));
                itemUIManager.addItem(items.Count - 1);
                Debug.Log("higher jump");
                itemForUs = true;
            }
            else if (rand < 0.70f)
            {
                otherPlayer.items.Add(new Item(0, 3, 5, 0, normalJumpHeight, normalSpeed));
                otherPlayer.itemUIManager.addItem(otherPlayer.items.Count - 1);
                Debug.Log("other player cant jump");
            }
            else if (rand < 0.85f)
            {
                otherPlayer.items.Add(new Item(0, 4, 7, 1, normalJumpHeight, normalSpeed * 0.5f));
                otherPlayer.itemUIManager.addItem(otherPlayer.items.Count - 1);
                Debug.Log("other player becomes slow");
            }
            else
            {
                otherPlayer.items.Add(new Item(0, 5, 7, 1, normalJumpHeight * 0.75f, normalSpeed));
                otherPlayer.itemUIManager.addItem(otherPlayer.items.Count - 1);
                Debug.Log("other player cant jump as high");
            }

            if (itemForUs)
            {
                Instantiate(ItemParticlesThisPlayer, (Vector3)(Vector2)inPos + new Vector3(0.5f, 0, 0), Quaternion.identity);
            }
            else {
                Instantiate(ItemParticlesOtherPlayer,(Vector3)(Vector2)inPos+new Vector3(0.5f,0,0), Quaternion.identity);
            }

        }
    
    
    }

    void itemUpdate()
    {
        speed = normalSpeed;
        maxJumps = 1;
        jumpHeight = normalJumpHeight;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].effectTimer > 0)
            {
                if (items[i].type == 0)
                {
                    speed = items[i].speed;
                    maxJumps = items[i].maxJumps;
                    jumpHeight = items[i].jumpHeight;
                    items[i].effectTimer -= Time.deltaTime;
                }
            }
            else
            {
                itemUIManager.removeItem(i);
                items.RemoveAt(i);
                i--;
            }
        }
    }

    void checkPointCheck(Vector2Int newCheckPointPos)
    {
        bool hasBeen = false;

        for (int i = 0; i < checkPointHits.Count; i++)
        {
            if (Vector2.Distance(fixedToInt(checkPointHits[i]), fixedToInt(newCheckPointPos)) < 5)
            {
                hasBeen = true;
            }
        }

        if (!hasBeen)
        {
            Instantiate(checkPointParticles, fixedToFloating(newCheckPointPos), Quaternion.identity);
            checkPointPos = newCheckPointPos + floatingToFixed(new Vector2(0, 0.5f));
            checkPointHits.Add(checkPointPos);
            levelManager.getLevel(fixedToFloating(newCheckPointPos)).checkPointHits++;
        }
    }

    float WaterDepth()
    {
        if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0.5f, 0.0f)) == 2)
        {
            //depth estimate
            float depth = 0;
            for (int i = 0; i < 10; i++)
            {
                if (levelManager.getTile(fixedToFloating(pos) + new Vector2(0.5f, Mathf.Pow(1.5f, i) * 0.1f)) == 2)
                {
                    depth = Mathf.Pow(1.5f, i) * 0.1f;
                }
                else
                {
                    depth = (Mathf.Pow(1.5f, i - 1) * 0.1f + Mathf.Pow(1.5f, i) * 0.1f) / 2.0f;
                    i = 11;
                }
            }
            return depth;
        }
        else { return 0; }
    }

    void Died()
    {
        pos = checkPointPos;
        vel = new Vector2Int(0, 0);
        maxJumps = 1;
        jumpHeight = 4.5f;
    }


    void updateAnimations()
    {
        AnimationFrameCountdown -= Time.deltaTime;
        if (AnimationFrameCountdown < 0)
        {
            AnimationFrameCountdown = 1.0f / AnimationFrameRate;
            AnimationFrameCount++;
            AnimationFrameCount = AnimationFrameCount % 4;
        }
        if (Mathf.Abs(fixedToFloating(vel.x)) > 0.25f)
        {
            if (grounded || inWater)
            {
                spriteRenderer.sprite = playerSprites[AnimationFrameCount];
            }
            else
            {
                spriteRenderer.sprite = playerSprites[1];
            }
        }
        else
        {
            AnimationFrameCountdown = 1.0f / AnimationFrameRate;
            AnimationFrameCount = 0;
            spriteRenderer.sprite = playerSprites[4];
        }
        spriteRenderer.flipX = !facing;
    }

    float fixedToFloating(int input)
    {
        float output = input;
        output /= 65536.0f;
        return output;
    }
    int floatingToFixed(float input)
    {
        input *= 65536.0f;
        return (int)input;
    }



    Vector2 fixedToFloating(Vector2Int input)
    {
        Vector2 output = input;
        output /= 65536.0f;
        return output;
    }
    Vector2Int floatingToFixed(Vector2 input)
    {
        input *= 65536.0f;
        return new Vector2Int((int)input.x, (int)input.y);
    }

    Vector3 fixedToFloating(Vector3Int input)
    {
        Vector3 output = input;
        output /= 65536.0f;
        return output;
    }
    Vector3Int floatingToFixed(Vector3 input)
    {
        input *= 65536.0f;
        return new Vector3Int((int)input.x, (int)input.y, (int)input.z);
    }
    int fixedFloor(int input)
    {
        return (input >> 16) << 16;
    }

    int fixedPointMult(int a, int b)
    {
        int sign = (int)(Mathf.Sign(a) * Mathf.Sign(b));
        a = Mathf.Abs(a);
        b = Mathf.Abs(b);
        uint byte1 = (((uint)a & 0b1111111111111111) * ((uint)b & 0b1111111111111111)) >> 16;
        uint byte2 = ((uint)a >> 16 & 0b1111111111111111) * ((uint)b & 0b1111111111111111);
        uint byte3 = ((uint)a & 0b1111111111111111) * ((uint)b >> 16 & 0b1111111111111111);
        uint byte4 = (((uint)a >> 16 & 0b1111111111111111) * (((uint)b >> 16) & 0b1111111111111111)) << 16;
        //Debug.Log(byte4+" "+fixedToFloating(byte4)+" "+ fixedToFloating(byte3) + " "+ fixedToFloating(byte2) + " "+ fixedToFloating(byte1));
        return sign * ((int)(byte4) + (int)(byte3) + (int)(byte2) + (int)(byte1));

    }
    Vector2Int fixedPointMult(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(fixedPointMult(a.x, b.x), fixedPointMult(a.y, b.y));
    }
    Vector2Int fixedPointMult(Vector2Int a, int b)
    {
        return new Vector2Int(fixedPointMult(a.x, b), fixedPointMult(a.y, b));
    }

    Vector2Int fixedToInt(Vector2Int input)
    {
        return new Vector2Int(input.x >> 16, input.y >> 16);
    }
    Vector2Int intToFixed(Vector2Int input)
    {
        return new Vector2Int(input.x << 16, input.y << 16);
    }

    bool AABB(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
    {
        drawDebugRect(aMin, aMax);
        drawDebugRect(bMin, bMax);
        return aMin.x <= bMax.x && aMax.x >= bMin.x &&
            aMin.y <= bMax.y && aMax.y >= bMin.y;
    }

    void drawDebugRect(Vector2 aMin, Vector2 aMax)
    {
        Debug.DrawLine(aMin, new Vector3(aMin.x, aMax.y));
        Debug.DrawLine(aMin, new Vector3(aMax.x, aMin.y));
        Debug.DrawLine(aMax, new Vector3(aMin.x, aMax.y));
        Debug.DrawLine(aMax, new Vector3(aMax.x, aMin.y));
    }

}


