using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;

public class SnowmanManager : MonoBehaviour
{
    [SerializeField] public Camera _camera;
    public UnityEvent<bool> snowmanCameraActivateEvent;
    Outline currentOutline;
    [SerializeField] private LayerMask terrainBoundariesMask;
    [SerializeField] private Cinemachine.CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private SnowmanCamera snowmanCamera;
    [SerializeField] private CameraControls playerCamera;
    [SerializeField] private InputActionReference performAction;
    [SerializeField] private InputActionReference changeSnowmanCam;
    public Transform dropPoints;
    public LayerMask snowmanMask;
    float _snowmanDropTime = 1;
    public float snowmanDropTime { get => _snowmanDropTime; private set { _snowmanDropTime = value; } }
    public float lastDroppedTime { get; private set; }

    public static SnowmanManager instance;
    List<Snowman> snowmen;
    List<Snowman> randomUnlockSnowmen;
    int index = 0;
    Snowman currentViewSnowman;
    void Awake()
    {
        lastDroppedTime = Time.time;
        Snowman.snowmanCreatedEvent += AddSnowman;
        snowmen = new List<Snowman>();
        randomUnlockSnowmen = new List<Snowman>(Resources.LoadAll<Snowman>("Prefabs/SnowmanPrefabs"));
        foreach(Snowman snowman in randomUnlockSnowmen)
        {
            snowman.description.unlocked = false;
            if (!snowman.description.randomUnlock)
            {
                randomUnlockSnowmen.Remove(snowman);
            }
        }
        snowmanCamera.gameObject.SetActive(false);
        instance = this;
    }

    public bool PlayerCameraActive()
    {
        return playerCamera.gameObject.activeSelf;
    }

    public void AddSnowman(Snowman snowman)
    {
        snowmen.Add(snowman);
        Snowman randomSnowmanByDescription = randomUnlockSnowmen.Find(x => x.description == snowman.description);
        randomUnlockSnowmen.Remove(randomSnowmanByDescription);
    }

    public Snowman GetRandomSnowmanUnlock()
    {
        if (randomUnlockSnowmen.Count == 0) return null;
        int randomIndex = Random.Range(0, randomUnlockSnowmen.Count);
        Snowman pulledSnowman = randomUnlockSnowmen[randomIndex];
        randomUnlockSnowmen.RemoveAt(randomIndex);
        return pulledSnowman;
    }

    public void setCurrentLookIndex(int index)
    {
        this.index = Mathf.Clamp(index, 0, snowmen.Count-1);
    }

    public void increaseLookIndex(int amount)
    {
        index += amount;
        if(index < 0)
        {
            index = snowmen.Count - 1;
        } else if(index > snowmen.Count - 1)
        {
            index = 0;
        }
    }

    public Snowman GetCurrentSnowman()
    {
        if(snowmen.Count != 0 && index < snowmen.Count)
        {
            return snowmen[index];
        } else
        {
            return null;
        }
    }

    public bool CheckCurrentSnowman(Snowman snowman)
    {
        if(snowman.TryGetComponent(out Outline outline))
        {
            return outline == currentOutline;
        } else
        {
            return false;
        }
    }

    int GetSnowmanIndex(Snowman snowman)
    {
        return snowmen.IndexOf(snowman);
    }

    void RemoveOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    void HoverSnowman()
    {
        if (PauseScript.isPaused()) return;
        if (EventSystem.current.IsPointerOverGameObject()) { RemoveOutline(); return; }
        RaycastHit info;
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = _camera.farClipPlane;
        Ray ray = new Ray(_camera.transform.position, _camera.ScreenToWorldPoint(mousePos));
        Debug.DrawLine(_camera.transform.position, _camera.ScreenToWorldPoint(mousePos));
        if (Physics.Raycast(_camera.transform.position, _camera.ScreenToWorldPoint(mousePos) - _camera.transform.position, out info, _camera.farClipPlane, snowmanMask))
        {
            if (currentViewSnowman == null || info.transform != currentViewSnowman.transform)
            {
                if (info.transform.TryGetComponent(out Outline outline))
                {
                    if (currentOutline != outline)
                    {
                        if (currentOutline != null) currentOutline.enabled = false;
                        currentOutline = outline;
                        currentOutline.enabled = true;
                    }
                }
            } else
            {
                RemoveOutline();
            }
        }
        else
        {
            RemoveOutline();
        }
    }

    public void SwitchCamera()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { RemoveOutline(); return; }
        if (currentOutline != null)
        {
            if (currentOutline.transform.TryGetComponent(out Snowman snowman))
            {
                index = snowmen.IndexOf(snowman);
                ActivateSnowmanCamera(snowman);
            }
            else
            {
                ActivatePlayerCamera();
            }
            RemoveOutline();
        } else
        {
            ActivatePlayerCamera();
        }
    }

    public void ActivateSnowmanCamera(Snowman snowman)
    {
        if (snowman == null) return;
        if (snowman == currentViewSnowman) { ActivatePlayerCamera(); return; }

        index = GetSnowmanIndex(snowman);
        currentViewSnowman = snowman;
        snowmanCamera.SetSnowmanTarget(snowman);
        snowmanCamera.ActivateCamera();
        snowman.OnViewEvent();
        snowmanCameraActivateEvent?.Invoke(true);
        playerCamera.gameObject.SetActive(false);

        snowmanCamera.gameObject.SetActive(true);
    }

    public void ActivatePlayerCamera()
    {
        if (playerCamera.gameObject.activeSelf) return;
        if (_camera == null) return;

        Vector3 snowmanPos = currentViewSnowman.transform.position + Vector3.up;
        Vector3 playerCameraPosition = _camera.transform.position - _camera.transform.forward * 2 + Vector3.up;
        //Move player back into bounds if out of bounds
        if (_camera != null)
        {
            Vector3 rayDir = (playerCameraPosition - snowmanPos);
            RaycastHit hit;
            if (Physics.Raycast(snowmanPos, rayDir.normalized, out hit, rayDir.magnitude, terrainBoundariesMask))
            {
                Debug.Log("Broke into wall after leaving camera");
                playerCameraPosition = hit.point + hit.normal;
            }
        }

        snowmanCamera.gameObject.SetActive(false);
        currentViewSnowman = null;

        playerCamera.transform.position = playerCameraPosition;
        snowmanCameraActivateEvent?.Invoke(false);
        playerCamera.gameObject.SetActive(true);
        snowmanCamera.DeactivateCameras();
    }

    public void ViewNextSnowman(float direction)
    {
        if (PauseScript.isPaused()) return;

        if (!snowmanCamera.gameObject.activeSelf)
        {
            setCurrentLookIndex(0);
        }
        else
        {
            increaseLookIndex(direction < 0 ? -1 : 1);
        }

        ActivateSnowmanCamera(GetCurrentSnowman());
    }

    public float TimeSinceDrop()
    {
        if(randomUnlockSnowmen.Count == 0)
        {
            return 0;
        } else
        {
            return Time.time - lastDroppedTime;
        }
    }

    void CheckForSnowmanDrop()
    {
        if(TimeSinceDrop() > snowmanDropTime)
        {
            Vector3 randomOffset = new Vector3((Random.value * 2) - 1, 0, (Random.value * 2) - 1);
            Instantiate(GetRandomSnowmanUnlock(), dropPoints.GetChild(Random.Range(0, dropPoints.childCount)).position + randomOffset, Quaternion.identity);
            if(randomUnlockSnowmen.Count == 0)
            {
                lastDroppedTime = 0;
            } else
            {
                lastDroppedTime = Time.time;
            }
        }
    }

    // Update is called once per frame
    float changeSnowmanIndex;
    void Update()
    {
        changeSnowmanIndex = changeSnowmanCam.action.ReadValue<float>();

        CheckForSnowmanDrop();
        HoverSnowman();
        if (performAction.action.triggered)
        {
            SwitchCamera();
        }
        if (changeSnowmanCam.action.WasPressedThisFrame() && changeSnowmanIndex != 0)
        {
            ViewNextSnowman(changeSnowmanIndex);
        }
    }
}
