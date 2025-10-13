using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video; //VideoPlayer기능을 사용하기 위한 네임스페이스

public class Video360Play : MonoBehaviour
{
    VideoPlayer vp;
    public VideoClip[] vcList;
    // Start is called before the first frame update
    void Start()
    {
        vp=GetComponent<VideoPlayer>();
        vp.clip = vcList[0]; //첫번째 값을 임의로 넣어줌
        vp.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
