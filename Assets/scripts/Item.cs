using UnityEngine;


public class Item
{
    public int type; //0 affects stats 1 throwable
    public int itemID;
    public float effectTimer; //can be used for how long an affect lasts or for if the throwable has been used yet
    public int maxJumps;
    public float jumpHeight;
    public float speed;
    public float maxEffectTimer;
    public Item(int type,int itemID,float effectTimer,int maxJumps,float jumpHeight,float speed)
    {
        this.type = type;
        this.itemID = itemID;
        this.effectTimer = effectTimer;
        maxEffectTimer = effectTimer;
        this.maxJumps = maxJumps;
        this.jumpHeight = jumpHeight;  
        this.speed = speed;
    }
    
}
