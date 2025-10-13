using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //UI image기능을 사용하기 위한 네임스페이스
using UnityEngine.Video; //VideoPlayer를 제어하기 위한 네임스페이스


//카메라의 시선을 처리하기 위한 기능
public class GazePointerCtrl : MonoBehaviour
{
    public Transform uiCanvas;
    public Image gazeImg;
    Vector3 defaultScale; //ui를 오브젝트화 하기 때문에, 카메라와의 거리가 중요하다.
    public float uiScaleVal = 1f;

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = uiCanvas.localScale;  //오브젝트가 갖는 기본 스케일 값
    }

    // Update is called once per frame
    void Update()
    {
        //캔버스 오브젝트의 스케일을 거리에 따라서 조절한다.
        //1.카메라를 기준으로 전방 방향의 좌표를 구한다(각도정보,Roll Pitch, Yaw).
        Vector3 dir = transform.TransformPoint(Vector3.forward);
        //2.카메라를 기준으로 전방의 레이를 설정한다.
        Ray ray = new Ray(transform.position,dir); //위치와 방향정보
        RaycastHit hitInfo;//히트된 오브젝트의 정보를 담는다.
        //3.레이에 부딪힌 경우에는 거리 값을 이용해 uiCanvas의 크기를 조절한다.
        if(Physics.Raycast(ray,out hitInfo))
        {
            uiCanvas.localScale = defaultScale * uiScaleVal *hitInfo.distance; //hitInfo의 정보 중 거리를 가져옴
            uiCanvas.position=transform.forward*hitInfo.distance; //부딪힌 지점에 ui오브젝트 생성하게 됨
        }
        else
        {
            //4.레이에 아무것도 부딪히지 않으면 기본 스케일 값으로 uiCanvas의 크기를 조절
            uiCanvas.localScale = defaultScale * uiScaleVal ;
            uiCanvas.position=transform.forward + dir; //정면 방향의 위치 정보를 알 수 있음
        }
        //5.uiCanvas가 사용자를 바라볼 수 있도록 반전(원래는 뒤집어져있음)
        uiCanvas.forward =transform.forward; //*1;

    }
}
