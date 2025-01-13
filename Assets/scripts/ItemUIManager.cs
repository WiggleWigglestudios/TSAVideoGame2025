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
    // Audio
    public AudioSource audioSourceItem;
    public AudioClip itemActiveNoise;
    public AudioClip itemDeactiveNoise;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSourceItem = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector2 targetPos = new Vector3(100+i*120, 100);
            targetPos -= items[i].GetComponent<RectTransform>().anchoredPosition;
            targetPos*=Mathf.Pow(0.95f,Time.deltaTime)*Time.deltaTime;
            items[i].GetComponent<RectTransform>().anchoredPosition += targetPos;
            items[i].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            items[i].GetComponent<RectTransform>().localScale=new Vector3(1,1,1)* Mathf.Pow(Mathf.Max(player.items[i].effectTimer / player.items[i].maxEffectTimer,0),0.2f);
        }
    }

    public void addItem(int index)
    {
        items.Add(Instantiate(itemImagePrefab,transform));
        items[items.Count - 1].GetComponent<Image>().sprite= itemImages[player.items[index].itemID];
        items[items.Count - 1].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(100 + (items.Count-1) * 120, -100,0);

        // Audio
        Debug.Log("AudioSource:"+audioSourceItem+" itemActivate:"+itemActiveNoise);
        if (audioSourceItem != null && itemActiveNoise != null)
        {
            audioSourceItem.PlayOneShot(itemActiveNoise);
        }
    }
    public void removeItem( int index)
    {
        // Audio
        if (audioSourceItem != null && itemDeactiveNoise != null)
        {
            audioSourceItem.PlayOneShot(itemDeactiveNoise);
        }

        Destroy(items[index]);
        items.RemoveAt(index);
    }
}
