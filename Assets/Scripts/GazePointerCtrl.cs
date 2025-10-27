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
    Vector3 defaultScale; //ui를 오브젝트화 하기 때문에, 카메라와의 거리가 중요하다.자동으로 private이 됨
    public float uiScaleVal = 1f; //1f, 1.0f 둘 다 가능

    //시간표시기능 제작
    bool isHitObj; //인터랙션이 일어나는 오브젝트에 시선이 닿으면 true, 닿지 않으면 flase
    GameObject prevHitObj; //이전 프레임의 시선이 ㅓ물렀던 오브젝트 정보를 담기 위한 변수
    GameObject curHitObj; //현재 프레임의 시선이 머물렀던 오ㅡ젝트 저오를 담기 위한 변수
    float curGazeTime = 0 ; //시선이 머무르는 시간을 저장하기 위한 변수
    public float gazeChargeTime = 3f; //게이지가 차는 시간을 체크하기 위한 기준 시간 3초(필요에 따라 수정)


    public Video360Play vp360;

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = uiCanvas.localScale;  //오브젝트가 갖는 기본 스케일 값
        curGazeTime = 0; //시선을 유지하는지 체크하기 위한 변수를 초기화
    }

    // Update is called once per frame
    void Update()
    {
        //캔버스 오브젝트의 스케일을 거리에 따라서 조절한다.
        //1.카메라를 기준으로 전방 방향의 좌표를 구한다(각도정보,Roll Pitch, Yaw).
        Vector3 dir = transform.TransformPoint(Vector3.forward);
        //2.카메라를 기준으로 전방의 레이를 설정한다.
        Ray ray = new Ray(transform.position, dir); //위치와 방향정보
        RaycastHit hitInfo;//히트된 오브젝트의 정보를 담는다.
        //3.레이에 부딪힌 경우에는 거리 값을 이용해 uiCanvas의 크기를 조절한다.
        if (Physics.Raycast(ray, out hitInfo))
        {
            uiCanvas.localScale = defaultScale * uiScaleVal * hitInfo.distance; //hitInfo의 정보 중 거리를 가져옴
            uiCanvas.position = transform.forward * hitInfo.distance; //부딪힌 지점에 ui오브젝트 생성하게 됨
            if (hitInfo.transform.tag == "GazeObj")
            {
                isHitObj = true;
            }
            curHitObj = hitInfo.transform.gameObject;
            //내가 아닌 외부의 정보에 접근 시, 부품(Transform)을 먼저 받고 그 후에 gameObject로 접근해야 함
        }

        else
        {
            //4.레이에 아무것도 부딪히지 않으면 기본 스케일 값으로 uiCanvas의 크기를 조절
            uiCanvas.localScale = defaultScale * uiScaleVal;
            uiCanvas.position = transform.forward + dir; //정면 방향의 위치 정보를 알 수 있음
        }
        //5.uiCanvas가 사용자를 바라볼 수 있도록 반전(원래는 뒤집어져있음)
        uiCanvas.forward = transform.forward; //*1;


        if (isHitObj) //만약 GazeObj태그가 붙은 오브젝트에 시선이 머무르고 있다면
        {
            //데이터처리
            // if(isHitObj == prevHitObj)
            if (curHitObj == prevHitObj) //이전 프레임과 현재 프레임의 시선이 머문 오브젝트가 같다면
            {
                curGazeTime = curGazeTime + Time.deltaTime; //게이지 시간 누적
            }
            else
            {

                prevHitObj = curHitObj; //아니라면 이전 프레임의 오브젝트 정보를 현재 프레임의 오브젝트 정보로 변경
                curGazeTime = 0; //시간 초기화
            }

            HitObjChecker(curHitObj, true);

        }
        else //시선을 벗어났거나 GazeObj태그가 아니라면 시간을 초기화
        {

            if (prevHitObj != null) //직전에 바라보면 오브젝트가 있었다면
            {
                HitObjChecker(prevHitObj, false); //이전 프레임의 오브젝트에 대해서도 멈추게 처리
                prevHitObj = null; //이제 바라보는 물체 없으니 초기화
            }
            curGazeTime = 0; //시간 초기화

        }

        curGazeTime = Mathf.Clamp(curGazeTime, 0, gazeChargeTime); //시선이 머문 시간을 0과 최댓값(여기에선 3초) 사이로 한다.
        gazeImg.fillAmount = curGazeTime / gazeChargeTime; //0% ~100%

        //플래그 내림
        //리셋하지 않으면 시선을 떼도 isHitObj가 계속 true로 남아있게 됨
        //한곳만 제대로 보고 있다면 시간은 계속 누적되기때문에 걱정할 필요 없음
        isHitObj = false;
        curHitObj = null;


    }

    //위에서 무엇을 봤는지 결정한 것이고
    //이제는 무엇을 할건지 결정하는 함수
    void HitObjChecker(GameObject hitObj,bool isActive)
    {
        if (hitObj.GetComponent<VideoPlayer>()) //VideoFrame 컴포넌트가 붙어있는 오브젝트라면!!
        {
            if (isActive) //시선이 닿았다면!
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(true); //videoFrame의 영상을 재생
            }

            else //시선이 벗어났다면!
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(false); //videoFrame의 영상을 정지
            }

        }
        
        //정해진 시간이 되면 360스피어에 특정 클립 번호를 전달해 플레이한다.
        if(gazeImg.fillAmount >=1) //게이지가 다 찼다면 360비디오 재생
        {
            // if (hitObj.name.Contains("Right"))
            // {
            //     vp360.SwapVideoClip(true);
            // }
            // else if (hitObj.name.Contains("Left")) //이름에 포함하는지 -->name.contains
            // {
            //     vp360.SwapVideoClip(false);
            // }
            // else
            // {
            //     vp360.SetVideoPlay(hitObj.transform.GetSiblingIndex());
            // }
            vp360.SetVideoPlay(hitObj.transform.GetSiblingIndex()); //GetSiblingIndex : 자식 오브젝트들 중에서 몇 번째인지를 인덱스로 알려주는 함수
        }
        
    }

}
//메모 : GazeObj태그가 잘 되어있는지 확인하기. 현재 상태는 안되어 있음.영상에 태그가 있어야 하는 것으로 추정.근데 sphere에 있음
//중간고사는 VR미포함

//Ray,RayCast,pool을 유심히 보기,자주 사용하는 함수, 키보드 키 받아오는 함수 같은 것
//1번 프로잭트와 2번 프로젝트를 합친 것이 실습 시험
//활용은 space를 알파벳 a로 입력받기 정도
//코드에서용어 물어보는 20문제(뭘 할려면 어떤 거 추가해야 하는가), 오전 10시, 주관식, 서술형,30분 이론시험, 1시간 실습시험, 실습때는 GPT만 막음, 현장 채점, GIT사용 가능,인터넷 서칭 가능