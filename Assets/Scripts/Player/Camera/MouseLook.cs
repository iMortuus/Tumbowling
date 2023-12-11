using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Mouse Settings")]
    [Range(0 , 3f)]
    public static float mouseSensitivity;
    [SerializeField] private float mouseSensitivityVisualizer;

    public Vector2 xRotation;
    public Transform playerBody;

   [SerializeField]  private float m_Angle;
    [SerializeField] private float m_HeadAngle;
    [SerializeField] private Quaternion m_NetworkRotation;
    [SerializeField] private Quaternion m_NetworkHeadRotation;
    bool m_firstTake = false;

    [Header("Script References")]
    public Camera myCam;
    public Transform headHolder;
    public Transform head;
    public PlayerMovement playerMovementScript;
    public PlayerStateHandler playerStateHandler;
    public PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;
        headHolder = transform.Find("HeadHolder");
        head = transform.Find("HeadHolder/Head");
        myCam.gameObject.SetActive(photonView.IsMine); 
        m_NetworkRotation = Quaternion.identity;
        m_NetworkHeadRotation = Quaternion.identity;
        m_firstTake = true;
    }


    private void Update()
    {
        if(!photonView.IsMine && playerStateHandler.states != PlayerStates.Dead){
            if(playerStateHandler.states != PlayerStates.Sliding){
                headHolder.localRotation = Quaternion.RotateTowards(headHolder.localRotation, m_NetworkRotation, m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }else{
                headHolder.rotation = Quaternion.RotateTowards(headHolder.rotation, m_NetworkRotation, m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
            head.localRotation = Quaternion.RotateTowards(head.localRotation, m_NetworkHeadRotation, m_HeadAngle * (1.0f / PhotonNetwork.SerializationRate));
        }else{
            MouseLookHandler();
            mouseSensitivity = mouseSensitivityVisualizer;
        }
        
    }

    void MouseLookHandler(){
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseAxis *= (mouseSensitivity * 200) * Time.deltaTime;

        xRotation += mouseAxis;
        xRotation.y = Mathf.Clamp(xRotation.y, -90f, 90f);
        
        if(playerStateHandler.states != PlayerStates.Sliding){
            headHolder.localRotation = Quaternion.AngleAxis(xRotation.x, Vector3.zero);
            playerBody.rotation = Quaternion.AngleAxis(xRotation.x, Vector3.up);
        }else{
            headHolder.rotation = Quaternion.AngleAxis(xRotation.x, Vector3.zero);
            //xRotation.x = Mathf.Clamp(xRotation.x, -95f, 95f);
            headHolder.rotation = Quaternion.AngleAxis(xRotation.x, Vector3.up);
        }
        head.localRotation = Quaternion.AngleAxis(-xRotation.y, Vector3.right);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting){
            if(playerStateHandler.states != PlayerStates.Sliding){
                stream.SendNext(headHolder.localRotation);
            }else{
                stream.SendNext(headHolder.rotation);
            }
            stream.SendNext(head.localRotation);
        }

        else if(stream.IsReading){
            m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            m_NetworkHeadRotation = (Quaternion)stream.ReceiveNext();
            if(playerStateHandler.states != PlayerStates.Sliding){
                m_Angle = Quaternion.Angle(headHolder.localRotation, m_NetworkRotation);
            }else{
                m_Angle = Quaternion.Angle(headHolder.rotation, m_NetworkRotation);
            }
            m_HeadAngle = Quaternion.Angle(head.localRotation, m_NetworkHeadRotation);
        }
    }
}
