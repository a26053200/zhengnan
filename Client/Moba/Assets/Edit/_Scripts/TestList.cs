using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestList : MonoBehaviour
{
    private ScrollList scrollList;
    // Use this for initialization
    void Start()
    {
        scrollList = gameObject.GetComponent<ScrollList>();

        scrollList.onItemRender = onItemRender;
        scrollList.ChildCount = 100;
        //scrollList.ReBuild();
        GameObject orgItem = GameObject.Find("SrvItem");
        orgItem.SetActive(false);
        //scrollList.SetItem(orgItem);
        StartCoroutine(InitList());

    }
    IEnumerator InitList()
    {
        yield return new WaitForEndOfFrame();
        scrollList.MaxPerLine = 1;
    }

    void onItemRender(int index, Transform child)
    {
        child.gameObject.SetActive(true);
        child.gameObject.SetText("Text", index.ToString());
        child.gameObject.SetText("Toggle/Label", "btn" + index.ToString());
    }
}
