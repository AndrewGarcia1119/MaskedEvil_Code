using System.Collections;
using System.Collections.Generic;
using AI;
using Unity.VisualScripting;
using UnityEngine;
using System;

namespace Player
{
    //This class currently requires that it is on a gameobject with the camera component
    [RequireComponent(typeof(Camera))]
    public class Sniper : MonoBehaviour
    {
        [Header("Aiming")]
        // The current divider of the default FOV. If preferred, we can remove this variable to change it with the literal desired FOV
        [SerializeField]
        float zoomFOV = 15f;
        //This is meant for the scope sprite when looking through the scope.
        [SerializeField]
        GameObject sniperScopeCanvas = null;
        //To make sure you can mark a character without having them having to be in the exact center of your screen, this field will help give a radius of how far from the crosshair a character can be and still get marked
        [SerializeField]
        float markCharacterAimTolerance = 1f;

        [Header("Animations")]
        [SerializeField]
        private Animator sniperAnimator = null;
        [SerializeField]
        private string aimAnimName = "Aim";
        [SerializeField]
        private string unaimAnimName = "Unaim";

        [Header("Sounds")]
        [SerializeField]
        private AudioClip sniperSound = null;

        [Header("Shooting")]
        [SerializeField]
        private float shootingCooldown = 1f;

        [HideInInspector]
        public bool hasAmmo = true;

        //default camera FOV, usually 60
        private float defaultFOV;
        //reference to camera component
        private Camera cam;
        //this variable determines if the player is able to shoot, this variable is meant to be used to check if the player is aiming before shooting. The player should only be able to shoot when aiming
        private bool ableToShoot;
        private bool justEnabled;
        private bool started;
        private float shootTimer;
        private bool endedPause = false;


        //event that occurs each time the player shoots. bool set to true if player successfully hits an AI.
        public static event Action<bool> onPlayerShoot;

        void Start()
        {
            cam = GetComponent<Camera>();
            defaultFOV = cam.fieldOfView;
            sniperAnimator.Play(unaimAnimName, 0, 1);
            started = true;
        }

        private void OnEnable()
        {
            if (!started) return;
            justEnabled = true;
            if (sniperAnimator)
            {
                sniperAnimator.Play(unaimAnimName, 0, 1);
                EndAim();
            }
        }

        private void OnDisable()
        {
            started = true;
        }
        // Update is called once per frame
        void Update()
        {
            if (Time.timeScale == 0)
            {
                endedPause = true;
                return;
            }
            if (sniperAnimator)
            {
                if (!endedPause)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        justEnabled = false;
                        sniperAnimator.Play(aimAnimName, 0, Mathf.Max(0, 1 - sniperAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
                    }
                    else if (Input.GetMouseButtonUp(1))
                    {
                        if (!justEnabled)
                        {
                            sniperAnimator.Play(unaimAnimName, 0, Mathf.Max(0, 1 - sniperAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
                        }
                    }
                }
                else
                {
                    endedPause = false;
                    if (Input.GetMouseButton(1))
                    {
                        justEnabled = false;
                        if (!sniperAnimator.GetCurrentAnimatorStateInfo(0).IsName(aimAnimName))
                        {
                            sniperAnimator.Play(aimAnimName, 0, Mathf.Max(0, 1 - sniperAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
                        }
                        else
                        {
                            sniperAnimator.Play(aimAnimName, 0, Mathf.Max(0, sniperAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
                        }
                    }
                    else if (!Input.GetMouseButton(1))
                    {
                        if (!justEnabled)
                        {
                            if (!sniperAnimator.GetCurrentAnimatorStateInfo(0).IsName(unaimAnimName))
                            {
                                sniperAnimator.Play(unaimAnimName, 0, Mathf.Max(0, 1 - sniperAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
                            }
                            else
                            {
                                sniperAnimator.Play(unaimAnimName, 0, Mathf.Max(0, sniperAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));
                            }
                        }
                    }
                }
            }
            else //This should only run if there is no animator component, more for testing purposes than anything
            {
                //If the mousebutton is held down, it will change FOV, and if there is an image attached to the script. It will also set it active at that moment. When the mousebutton is not held down it will disable the image and zoom out
                if (Input.GetMouseButton(1))
                {
                    Aim();
                }
                else
                {
                    EndAim();
                }
            }

            if (ableToShoot && Input.GetKeyDown(KeyCode.W))
            {
                ToggleMarkCharacter();
            }
            /*
            WARNING: THIS CODE ONLY RUNS WHEN ableToShoot it true, but this only happens when the animator calls the Aim and Unaim methods
            */
            //The player can only shoot if aiming
            if (shootTimer >= shootingCooldown && ableToShoot && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            {
                Shoot();
            }
            shootTimer += Time.deltaTime;
        }

        public void Aim()
        {
            if (sniperScopeCanvas)// && !sniperScopeCanvas.activeSelf)
            {
                sniperScopeCanvas.SetActive(true);
            }
            cam.fieldOfView = zoomFOV;
            ableToShoot = true;
        }

        public void EndAim()
        {
            if (sniperScopeCanvas) // && sniperScopeCanvas.activeSelf)
            {
                sniperScopeCanvas.SetActive(false);
            }
            if (cam)
            {
                cam.fieldOfView = defaultFOV;
            }
            ableToShoot = false;
        }


        private void Shoot()
        {
            shootTimer = 0f;
            if (!hasAmmo) return;
            if (Time.timeScale == 0) return;
            if (sniperSound)
            {
                AudioSource.PlayClipAtPoint(sniperSound, transform.position);
            }
            if (Physics.Raycast(GetScreenCenterRay(), out RaycastHit hit, 500f))
            {

                if (!hit.transform.TryGetComponent<IShootable>(out IShootable hitShootable))
                {
                    if ((hitShootable = hit.transform.GetComponentInParent<IShootable>()) == null)
                    {
                        hitShootable = hit.transform.GetComponentInChildren<IShootable>();
                    }
                }
                if (hitShootable != null)
                {
                    hitShootable.OnHit(true, gameObject.transform);
                    if (hitShootable is AIController)
                    {
                        onPlayerShoot?.Invoke(true);
                    }
                    else
                    {
                        onPlayerShoot?.Invoke(false);
                    }
                    return;
                }
                onPlayerShoot?.Invoke(false);
            }
            else
            {
                onPlayerShoot?.Invoke(false);
            }

        }

        private void ToggleMarkCharacter()
        {
            bool foundAIOnRaycast = false;
            if (Physics.Raycast(GetScreenCenterRay(), out RaycastHit hit, 500f))
            {

                if (!hit.transform.TryGetComponent<AIController>(out AIController hitAI))
                {
                    if ((hitAI = hit.transform.GetComponentInParent<AIController>()) == null)
                    {
                        hitAI = hit.transform.GetComponentInChildren<AIController>();
                    }
                }
                if (hitAI != null && !hitAI.isDead)
                {
                    hitAI.ToggleMark(!hitAI.IsMarked());
                    foundAIOnRaycast = true;
                }
            }
            if (!foundAIOnRaycast)
            {
                foreach (RaycastHit possibleHit in Physics.SphereCastAll(GetScreenCenterRay(), markCharacterAimTolerance, 500f))
                {
                    if (!possibleHit.transform.TryGetComponent<AIController>(out AIController hitAI))
                    {
                        if ((hitAI = possibleHit.transform.GetComponentInParent<AIController>()) == null)
                        {
                            hitAI = possibleHit.transform.GetComponentInChildren<AIController>();
                        }
                    }
                    if (hitAI != null)
                    {
                        hitAI.ToggleMark(!hitAI.IsMarked());
                        return;
                    }
                }
            }
        }

        private Ray GetScreenCenterRay()
        {
            return cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        }

    }
}