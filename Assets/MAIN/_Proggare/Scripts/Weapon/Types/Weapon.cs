﻿using UnityEngine;
using System.Collections;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

//Seb

public enum EffectType
{
    Fire = 0,
    Reload = 1,
    Pullout = 2,
    Equip = 3,
    Switch = 4,
}

public class Weapon : MonoBehaviour
{
    //[Header("OBS: <color=yellow>Variables</color> for each Weapon class are at the <color=yellow>bottom</color> under <color=#03fce3>Effects</color>")]
    //USING CUSTOM EDITOR SCRIPT

    //CUSTOM EDITOR
    public bool needsAmmo = true; //IS ALSO USED OUTSIDE OF THE CUSTOM EDITOR SCRIPT, aka this script
    //CUSTOM EDITOR

    //Configurable Perameters
    [Header("Ammo")]
    public int currentAmmo;
    public int totalAmmo = 90;
    public int magSize = 30;
    public int maxAmmo = 80;
    public int ammoFromPickup = 30;
    public TextMeshProUGUI ammoText;

    [Header("Reload")]
    public float waitBeforeReload = 0.5f;
    public float reloadTime = 3f;
    [Range(0, 1)] public float chanceToPlayReloadB;

    [Header("Delays")]
    public float firedelay = 3f;
    public float pullOutDelay = 0.5f;
    public float switchDelay = 0.5f;

    //[Header("Collision")]
    //USING CUSTOM EDITOR SCRIPT
    public LayerMask hitMask = 0;
    public float weaponRange = 100f;

    [Header("Screen Shake")]
    public bool shakeOnFire = true;
    public bool shakeOnHit = false;
    public float screenShakeDuration = 0.1f;
    public float screenShakeIntensity = 0.1f;

    [Header("Extra")]
    public Light flashLight;
    [Tooltip("Instantiate GameObjects on transform for a clearer Hierarchy")] public GameObject antiHierarchySpam;

    //[Header("<color=#03fce3> Effects")]
    //USING CUSTOM EDITOR SCRIPT
    [Tooltip("When calling an action, play the effects with the same EffectType")] public Effects[] effects;

    [System.Serializable]
    public struct Effects
    {
        public EffectType effectType;

        [Header("Audio & Animation Effects")]
        public Animator[] animators;

        [Space]

        public AudioClip audioClip;
        public Vector2 pitch;

        [Space]

        public FireEffects fireEffects;

        [System.Serializable]
        public struct FireEffects
        {
            //[Header("Muzzle Flash")]
            //USING CUSTOM EDITOR SCRIPT
            [Tooltip("Reference for enabling the muzzleFlash")] public Light[] muzzleLight;
            [Tooltip("Reference for random muzzleFlash particle rotation")] public ParticleSystem[] muzzleFlashParticle;
            [Tooltip("Time the flash is active")] public float muzzleTime;

            [Header("Hit Effects")]
            public ParticleSystem enemyHitParticle;
            public GameObject enviormentHitParticle;

            [Header("Extra")]
            public GameObject casing;
        }
    }

    //Protected Variables
    protected Camera mainCam = null;

    //Private Variabels
    private GameObject soundObject;

    private float currentFireDelay;
    private float currentPullOutDelay; 
    private float currentSwitchDelay; //Kan legit hämta med weaponManager.GetSwitchCurrentDelay();

    private bool reloading = false;
    private bool waitForReload = false;

    //Hold Fire
    private bool holdToFire = false;
    private bool buttonIsPressed = false;

    //Chaced References
    WeaponManager weaponManager;
    ScreenShake screenShake;
    ScoreManager scoreManager;
    Animator[] animator;

    //[SerializeField] private LayerMask headLayer;
    //[SerializeField] private LayerMask torsoLayer;
    //[SerializeField] private LayerMask leftArmLayer;
    //[SerializeField] private LayerMask rightArmLayer;

    #region Base Methods
    protected void Start()
    {
        StartCoroutine(EffectsCoroutine(EffectType.Pullout));

        scoreManager = FindFirstObjectByType<ScoreManager>();
        weaponManager = GetComponentInParent<WeaponManager>();

        if(weaponManager == null) { Debug.LogError("WeaponManager not found in parent object"); }

        animator = GetComponents<Animator>(); 
        animator = GetComponentsInChildren<Animator>();

        //Warning
        //Debug.LogWarning("Weapon.cs is a base class for all weapons, please use the derived classes instead.");

        //Check if pitch has a value
        foreach (var effect in effects)
        {
            if(effect.pitch == Vector2.zero)
            {
                Debug.Log("<color=Yellow>" + transform.name.ToString() + " <color=red>" + effect.effectType + " <color=white> Pitch has no value, No sound will be played");
            }
        }

        //Check if hitMask is set
        if (hitMask == 0) { Debug.Log("The <color=red>" + gameObject.name + "</color> has no <color=red>" + hitMask + "</color> selected"); }

        //Get Camera
        mainCam = Camera.main;
        screenShake = FindFirstObjectByType<ScreenShake>();

        //Set currentAmmo in start
        if (currentAmmo == 0)
        {
            currentAmmo = magSize;
        }
    }
    private void Update()
    {
        //Update the ammo UI when the gun is enabled
        if (gameObject.activeSelf && ammoText != null)
        {
            if (GetComponent<Shotgun>())
            {
                ammoText.text = totalAmmo.ToString();
            }
            else if (needsAmmo)
            {
                ammoText.text = currentAmmo + " / " + totalAmmo;
            }
            else
            {
                ammoText.text = "";
            }

            if (ammoText.GetComponentInParent<Animator>() != null && weaponManager.GetIsSwitching())
            {
                ammoText.GetComponentInParent<Animator>().SetBool("State", true);
            }
            else if (ammoText.GetComponentInParent<Animator>() != null && !weaponManager.GetIsSwitching())
            {
                ammoText.GetComponentInParent<Animator>().SetBool("State", false);
            }
        }

        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Cooldowns
        if (currentFireDelay > 0) { currentFireDelay -= Time.deltaTime; }
        else { currentFireDelay = 0; PlayAnimation(EffectType.Fire, "Fire", false); }

        if (weaponManager.GetIsSwitching()) { foreach (Animator animators in animator) { animators.SetTrigger("Switch"); currentSwitchDelay = weaponManager.GetSwitchCurrentDelay(); } }

        if (currentPullOutDelay > 0) { currentPullOutDelay -= Time.deltaTime; }
        else { currentPullOutDelay = 0; }

        //Ammo won't go over a certain limit
        if (totalAmmo > maxAmmo) { totalAmmo = maxAmmo; }

        //AUTO FIRE
        if(holdToFire && !buttonIsPressed || holdToFire && currentAmmo == 0) { foreach (Animator animator in animator) { if (animator != null) { animator.SetTrigger("StopFire"); } } }
    }
    public void OnEnable()
    {
        //// When the weapon is Enabled \\\\

        //Get Pullout animation lenght and set it to switch delay
        if (animator != null)
        {
            foreach (Animator animator in animator)
            {
                pullOutDelay = animator.GetCurrentAnimatorStateInfo(0).length;
            }

            currentPullOutDelay = pullOutDelay;
        }
        StartCoroutine(EffectsCoroutine(EffectType.Pullout));


        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Start Reload?
        if (reloading || currentAmmo == 0) { StartCoroutine(ReloadAfterPullout()); }
    }
    public void OnDisable()
    {
        StopAllWeaponCorutines();
    }
    #endregion

    #region Fire
    public virtual bool Fire()
    {
        //Check ammo requirements
        if (needsAmmo && currentAmmo == 0 && reloading == false && waitForReload == false && weaponManager.GetIsSwitching() == false) { /*Debug.Log("Ammo Needed"); */ return false; }
        else if (!needsAmmo && weaponManager.GetIsSwitching() == false) { /*Debug.Log("No Ammo Needed"); */ }

        //Check other requirements
        if (!holdToFire && currentFireDelay == 0 && reloading == false && waitForReload == false && gameObject.activeSelf == true && weaponManager.GetIsSwitching() == false && currentPullOutDelay <= 0)
        {
            //Screen Shake
            if (shakeOnFire) { screenShake.Shake(screenShakeDuration, screenShakeIntensity); }

            //Logic
            currentAmmo--;
            currentFireDelay = firedelay;
            if (currentAmmo == 0) { Reload(); }
            scoreManager.AddStatValues(StatType.ShotsFired, 1);


            //Effects
            PlayAnimation(EffectType.Fire, "Fire", true);
            PlaySound(EffectType.Fire);
            StartCoroutine(EffectsCoroutine(EffectType.Fire));

            return true;
        }
        else if (holdToFire && currentFireDelay == 0 && reloading == false && waitForReload == false && gameObject.activeSelf == true && weaponManager.GetIsSwitching() == false && currentPullOutDelay <= 0)
        {
            //Screen Shake
            if (shakeOnFire) { screenShake.Shake(screenShakeDuration, screenShakeIntensity); }

            //Logic
            currentAmmo--;
            currentFireDelay = firedelay;
            if (currentAmmo == 0) { Reload(); }

            //Effects
            if (buttonIsPressed) { PlayAnimation(EffectType.Fire, "Fire", true); }
            else { foreach (Animator animator in animator) { animator.SetTrigger("StopFire"); } }

            PlaySound(EffectType.Fire);
            StartCoroutine(EffectsCoroutine(EffectType.Fire));

            return true;
        }

        //false
        if (currentAmmo == 0) { Reload(); }
        return false;
    }
    public void CancelFire()
    {
        StopAllCoroutines();

        foreach (Animator animator in animator)
        {
            if (animator != null)
            {
                animator.SetTrigger("StopFire");
            }
        }

        // Disable muzzle flash particles and lights
        foreach (var effect in effects)
        {
            // Stop and clear all muzzle flash particles
            if (effect.fireEffects.muzzleFlashParticle != null)
            {
                foreach (var particleSystem in effect.fireEffects.muzzleFlashParticle)
                {
                    if (particleSystem != null)
                    {
                        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                }
            }

            // Disable all muzzle flash lights
            if (effect.fireEffects.muzzleLight != null)
            {
                foreach (var light in effect.fireEffects.muzzleLight)
                {
                    if (light != null)
                    {
                        light.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    #endregion
    
    #region Reload
    /// <summary>
    /// Try to initiate a realod
    /// </summary>
    public virtual bool Reload()
    {
        //Check if we can Reload
        if (currentAmmo == magSize || totalAmmo == 0 || reloading == true || waitForReload == true)
        {
            return false;
        }

        //Start Reload
        StartCoroutine(ReloadRoutine());
        return true;
    }
    /// <summary>
    /// Play the Reload animation and sound
    /// </summary>
    public IEnumerator ReloadRoutine()
    {
        //Reload Delay
        waitForReload = true;

        yield return new WaitForSeconds(waitBeforeReload);

        waitForReload = false;

        //Before Wait
        reloading = true;
        PlaySound(EffectType.Reload);
        StartCoroutine(EffectsCoroutine(EffectType.Reload));

        //Get Reload time
        if (animator != null)
        {
            foreach (Animator animator in animator)
            {
                yield return new WaitForSeconds(0.001f);
                reloadTime = animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }

        //Wait
        yield return new WaitForSeconds(reloadTime) ;

        FinishedReload();
    }
    /// <summary>
    /// When the Reload is finished
    /// </summary>
    public virtual bool FinishedReload()
    {
        //Max ammo needed
        int ammoToAdd = magSize - currentAmmo;

        //Check if we have enough ammo
        if (totalAmmo > ammoToAdd)
        {
            currentAmmo = magSize;
            totalAmmo -= ammoToAdd;
        }
        else
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }

        //Stop Effects
        StopCoroutine(EffectsCoroutine(EffectType.Reload));
        StopCoroutine(ReloadRoutine());
        PlayAnimation(EffectType.Reload, "Reload", false);

        reloading = false;
        return true;
    }
    IEnumerator ReloadAfterPullout()
    {
        yield return new WaitForSeconds(pullOutDelay);
        StartCoroutine(ReloadRoutine());
    }
    public void CancelReload()
    {
        //Stop Effects
        StopAllCoroutines();
        reloading = false;
        waitForReload = false;

        //Disable the reload sound
        Destroy(soundObject);
    }
    #endregion

    #region Effects
    /// <summary>
    /// Sets <paramref name="boolname"/> to <paramref name="animationBool"/> that are <paramref name="type"/> 
    /// </summary>
    public void PlayAnimation(EffectType type, string boolname, bool animationBool)
    {
        foreach (var animation in effects)
        {
            foreach (Animator animator in animation.animators)
            {
                animator.SetBool(boolname, animationBool);
            }
        }
    }
    /// <summary>
    /// Plays effects under "Effects" with the same <paramref name="type"/>
    /// </summary>
    public IEnumerator EffectsCoroutine(EffectType type)
    {
        foreach (var effect in effects)
        {
            if (EffectType.Fire == type)
            {
                //if()

                // Ensure the muzzleFlashParticle array is not null or empty
                if (effect.fireEffects.muzzleFlashParticle != null && effect.fireEffects.muzzleFlashParticle.Length > 0)
                {
                    foreach (var particleSystem in effect.fireEffects.muzzleFlashParticle)
                    {
                        if (particleSystem != null)
                        {
                            // Random rotation for each particle system
                            var mainModule = particleSystem.main;
                            mainModule.startRotation = Random.Range(0f, 365f);

                            // Play the particle system
                            particleSystem.Play();
                        }
                        else
                        {
                            Debug.LogWarning("A muzzle flash particle system is null in the effects array.");
                        }
                    }
                }

                // Ensure the muzzleLight array is not null or empty
                if (effect.fireEffects.muzzleLight != null && effect.fireEffects.muzzleLight.Length > 0)
                {
                    foreach (var light in effect.fireEffects.muzzleLight)
                    {
                        if (light != null)
                        {
                            // Enable the light
                            light.gameObject.SetActive(true);
                        }
                        else
                        {
                            Debug.LogWarning("A muzzle flash light is null in the effects array.");
                        }
                    }

                    // Wait for the muzzle flash duration
                    yield return new WaitForSeconds(effect.fireEffects.muzzleTime);

                    // Disable the light
                    foreach (var light in effect.fireEffects.muzzleLight)
                    {
                        if (light != null)
                        {
                            light.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else if (EffectType.Reload == type)
            {
                float f = Random.Range(0f, 1f);
                int reloadType = (f < chanceToPlayReloadB) ? 1 : 0;

                foreach (Animator animator in effect.animators)
                {
                    animator.SetInteger("ReloadType", reloadType);

                    PlayAnimation(EffectType.Reload, "Reload", true);
                }
            }
            else if (EffectType.Pullout == type) 
            {
                PlaySound(EffectType.Pullout);
            }
            else if (EffectType.Equip == type) { }
            else if (EffectType.Switch == type) 
            {
                foreach (Animator animator in effect.animators)
                {
                    animator.SetTrigger("Switch");
                    switchDelay = animator.GetCurrentAnimatorStateInfo(0).length;
                }
            }
        }
    }
    #endregion

    #region Audio Management
    /// <summary>
    /// Play the Audio under "Effects" that is of <paramref name="type"/>
    /// </summary>
    public void PlaySound(EffectType type)
    {
        foreach (var audio in effects)
        {
            if (audio.effectType == type && audio.audioClip != null)
            {
                PlaySoundEffect(audio.audioClip, type.ToString(), audio.pitch);
            }
        }
    }
    public void PlaySoundEffect(AudioClip clip, string type, Vector2 pitch)
    {
        //Create New Object
        GameObject soundFX = new(type + "Sound");

        //Set Transform
        soundFX.transform.position = Camera.main.transform.position;

        if (type != "Reload") { soundFX.transform.SetParent(antiHierarchySpam.transform); }
        else { soundFX.transform.SetParent(transform); }

        //Add Components
        soundFX.AddComponent<AudioSource>();
        soundFX.AddComponent<AudioPauseManager>();
        soundFX.AddComponent<Destroy>();

        //Edit Components
        soundFX.GetComponent<AudioSource>().playOnAwake = false;
        soundFX.GetComponent<AudioSource>().clip = clip;
        soundFX.GetComponent<AudioSource>().pitch = Random.Range(pitch.x, pitch.y);
        soundFX.GetComponent<AudioSource>().priority = 128 + currentAmmo;
        //soundFX.GetComponent<Destroy>().SetAliveTime(clip.length);

        //Destroy
        soundFX.GetComponent<AudioSource>().Play();

        soundObject = soundFX;
    }
    #endregion

    #region Raycast hit logic
    public void HitDetection(RaycastHit hit, Ray weaponRay, int damage)
    {
        //if(hit.transform.gameObject.tag == "Enemy Head" || hit.transform.tag == "Enemy Torso" || hit.transform.tag == "Enemy Left Arm" || hit.transform.tag == "Enemy Right Arm")
        //{
        //    hit.transform.SendMessage("PartDetected");
        //}

        /////////////////////////////Non Walkable///////////////////////////Walkable///////////////////////////////No Spawn Walkable///////////////
        if (hit.transform.gameObject.layer == 7 || hit.transform.gameObject.layer == 9 || hit.transform.gameObject.layer == 15)
        {
            Instantiate(effects[0].fireEffects.enviormentHitParticle, hit.point, Quaternion.LookRotation(hit.normal), antiHierarchySpam.transform);
        }
        else if (hit.transform.CompareTag("Grenade"))
        {
            hit.transform.GetComponent<Grenade>().Explode();
        }
        else if (hit.transform.CompareTag("Enemies"))
        {
            Instantiate(effects[0].fireEffects.enemyHitParticle, hit.point, Quaternion.LookRotation(hit.normal), antiHierarchySpam.transform);

            EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();

            //Debug.Log("Hit object with tag " + hit.transform.tag);

            if (enemy != null)
            {
                scoreManager.AddStatValues(StatType.ShotsHit, 1);
                scoreManager.AddStatValues(StatType.DamageDealt, damage);

                enemy.ApplyDamage(damage);
            }

            /*if (hit.transform.CompareTag("Enemy Head"))
            //{
            //    //EnemyScript enemy = hit.transform.GetComponentInParent<EnemyScript>();
            //    if (enemy != null)
            //    {
            //        enemy.ApplyDamageHead(pelletDamage);
            //        Debug.Log("Head took " + pelletDamage);
            //    }
            //}
            //else if (hit.transform.CompareTag("Enemy Torso"))
            //{
            //    //EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
            //    if (enemy != null)
            //    {
            //        enemy.ApplyTorsoDamage(pelletDamage);
            //        Debug.Log("Body took " + pelletDamage);
            //    }
            //}
            //else if (hit.transform.CompareTag("Enemy Left Arm"))
            //{
            //    //EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
            //    if (enemy != null)
            //    {
            //        enemy.ApplyLeftArmDamage(pelletDamage);
            //        Debug.Log("Left arm took " + pelletDamage);
            //    }
            //}
            //else if (hit.transform.CompareTag("Enemy Right Arm"))
            //{
            //    //EnemyScript enemy = hit.transform.GetComponent<EnemyScript>();
            //    if (enemy != null)
            //    {
            //        enemy.ApplyRightArmDamage(pelletDamage);
            //        Debug.Log("Right arm took " + pelletDamage);
            //    }
            //}*/
        }
        else if (hit.transform.CompareTag("Plank") && hit.transform.GetComponent<Plank>())
        {
            Plank plank = hit.transform.GetComponent<Plank>();
            plank.BreakPlanks(weaponRay.direction, 1, 15);
        }
        else
        {
            //Instantiate(temporaryHitParticel, hitPosition, hitRotation, antiHierarchySpam.transform);
        }
    }
    #endregion

    #region Get / Set
    public bool SetButtonPressed(bool value)
    {
        buttonIsPressed = value;
        return buttonIsPressed;
    }

    public bool GetReloading()
    {
        return reloading;
    }
    public float GetCurrentFireDelay()
    {
        return currentFireDelay;
    }
    public float GetFireRate()
    {
        return firedelay;
    }

    //Automatic Fire Get Set Bool. Set in Start 
    public bool GetIsHeldToFire()
    {
        return holdToFire;
    }
    public bool SetIsHoldToFire(bool value)
    {
        holdToFire = value;
        return holdToFire;
    }

    //Switch
    public float GetSwitchDelay()
    {
        return switchDelay;
    }
    #endregion

    #region Other
    //Used in loot drops
    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
    }

    public void StartShake(float duration, float intensity)
    {
        screenShake.Shake(duration, intensity);
    }

    //Used in WeaponManager
    public void StopAllWeaponCorutines()
    {
        StopAllCoroutines();
    }
    public void UpdateSwitchDelay()
    {
        if (animator != null)
        {
            foreach (Animator animators in animator) { switchDelay = animators.GetCurrentAnimatorStateInfo(0).length; }
        }
    }

    //Draw Gizmos
    private void OnDrawGizmosSelected()
    {
        // Draw a line in the scene view to visualize the weapon range
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + transform.forward * weaponRange);
    }
    #endregion
}