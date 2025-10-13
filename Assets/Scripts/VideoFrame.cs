using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//중요
using UnityEngine.Video;//비디오플레이어 기능을 사용하기 위한 네임스페이스 **

public class VideoFrame : MonoBehaviour
{
    //VideoPlayer컴포넌트
    VideoPlayer vp;
        
    // Start is called before the first frame update
    void Start()
    {
        vp = GetComponent<VideoPlayer>();
        //영상 재생을 멈춤
        vp.Stop();//Video Frame안에 있는 video
    }

    // Update is called once per frame
    void Update()
    {

        //영상재생의 조건(스페이스바)
        if(Input.GetKeyDown("space")){
            if(vp.isPlaying)
            {
                vp.Pause();
            }
            else
            {
                vp.Play();
            }
            
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            vp.Stop();
        }
        
    }

    public void CheckVideoFrame(bool Checker)
    {
        if(Checker)
        {
            if(!vp.isPlaying)
            {
                vp.Play();
            }

        }
        else
        {
            vp.Stop();
        }
    }
}
