using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tween : MonoBehaviour
{
    private GameObject mGo = null;
    // Start is called before the first frame update
    void Start()
    {
       this.mGo = GameObject.Find("CubeTween");
    }

    // Update is called once per frame
    void Update()
    {
        this.mGo.transform.position = new Vector3(this.mGo.transform.position.x + 0.1f,
            this.mGo.transform.position.y, this.mGo.transform.position.z);
    }
}
