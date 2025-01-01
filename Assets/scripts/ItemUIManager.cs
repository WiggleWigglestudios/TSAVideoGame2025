using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
public class ItemUIManager : MonoBehaviour
{
    public GameObject itemImagePrefab;
    public Player player;
    public List<GameObject> items=new List<GameObject>();
    public Sprite[] itemImages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 targetPos = new Vector3(100+i*120, 100, 0);
            targetPos += items[i].GetComponent<RectTransform>().position;
            targetPos*=Mathf.Pow(0.95f,Time.deltaTime);
            items[i].GetComponent<RectTransform>().position -= targetPos;
            items[i].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            items[i].GetComponent<RectTransform>().localScale=new Vector3(1,1,1)* player.items[i].effectTimer / player.items[i].maxEffectTimer;
        }
    }

    public void addItem(int index)
    {
        items.Add(Instantiate(itemImagePrefab,transform));
        items[items.Count - 1].GetComponent<Image>().sprite= itemImages[player.items[index].itemID];
    }
    public void removeItem( int index)
    {
        items.RemoveAt(index);
    }
}
