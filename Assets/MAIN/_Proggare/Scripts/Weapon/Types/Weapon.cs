using UnityEngine;
using System.Collections;
using TMPro;

//Seb

public enum EffectType
{
    fire = 0,
    reload = 1,
    pullout = 2,
    equip = 3,
}

public class Weapon : MonoBehaviour
{
    [Header("OBS: <color=yellow> Variabels </color> for each Weapon class are at the <color=yellow> bottom </color> under <color=#03fce3>Effects")]

    //Configurable Perameters
    [Header("Ammo")]
    public int currentAmmo;
    public int totalAmmo = 90;
    public int magSize = 30;
    public int maxAmmo = 80;
    public TextMeshProUGUI ammoText;

    [Header("Delays")]
    public float waitBeforeReload = 0.5f;
    public float reloadTime = 3f;
    public float firedelay = 3f;

    [Header("Extra")]
    public LayerMask hitMask = 0;
    public float weaponRange = 100f;
    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] public GameObject antiHierarchySpam;

    [Header("Screen Shake")]
    public float screenShakeDuration = 0.1f;
    public float screenShakeIntensity = 0.1f;

    [Header("<color=#03fce3> Effects")]
    [Tooltip("When calling a action play the effects with the same EffectType")] public Effects[] effects;

    [System.Serializable]
    public struct Effects
    {
        [Header("Audio & Animation Effects")]
        public EffectType effectType;
        public Animator animator;

        [Space]

        public AudioClip audioClip;
        public Vector2 pitch;

        [Space]

        public FireEffects fireEffects;

        [System.Serializable]
        public struct FireEffects
        {
            [Header("Muzzle Flash")]
            [Tooltip("Reference for enabling the muzzleFlash")] public Light[] muzzleLight;
            [Tooltip("Reference for random muzzleFlash particle rotation")] public ParticleSystem[] muzzleFlashParticle;
            [Tooltip("MAN FIGURE IT OUT")] public float muzzleTime;

            [Space]

            public Light flashLight;
            public GameObject casing;
            public ParticleSystem enemyHitParticle;
            public ParticleSystem enviormentHitParticle;
        }
    }

    //Protected Variables
    protected Camera mainCam = null;

    //Private Variabels
    private float currentFireDelay;
    private bool reloading = false;
    private bool waitForReload = false;

    private bool holdToFire = false;

    //Chaced References
    ScreenShake screenShake;

    #region Base Methods
    protected void Start()
    {
        //Warning
        Debug.LogWarning("Weapon.cs is a base class for all weapons, please use the derived classes instead.");
        
        foreach (var effect in effects)
        {
            if(effect.pitch == Vector2.zero)
            {
                Debug.Log("<color=Yellow>" + transform.name.ToString() + " <color=red>" + effect.effectType + " <color=white> Pitch has no value, No sound will be played");
            }
        }

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
            else
            {
                ammoText.text = currentAmmo + " / " + totalAmmo;
            }
        }
        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Cooldowns
        if (currentFireDelay > 0) { currentFireDelay -= Time.deltaTime; }
        else { currentFireDelay = 0; PlayAnimation(EffectType.fire, "Fire", false); }

        //Ammo won't go over a certain limit
        if (totalAmmo > maxAmmo) { totalAmmo = maxAmmo; }
    }
    public void OnEnable()
    {
        //// When the weapon is Enabled \\\\

        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Start reload?
        if (reloading || currentAmmo == 0) { StartCoroutine(ReloadRoutine()); }
    }
    public void OnDisable()
    {
        StopAllWeaponCorutines();
    }
    #endregion

    
    public virtual bool Fire()
    {
        //true
        if(currentAmmo > 0 && currentFireDelay == 0 && reloading == false && waitForReload == false && gameObject.activeSelf == true)
        {
            //Screen Shake
            screenShake.Shake(screenShakeDuration, screenShakeIntensity);

            //Logic
            currentAmmo--;
            currentFireDelay = firedelay;
            if (currentAmmo == 0) { Reload(); }

            //Effects
            PlayAnimation(EffectType.fire, "Fire", true);
            PlaySound(EffectType.fire);
            StartCoroutine(EffectsCoroutine(EffectType.fire));

            return true;
        }

        //false
        if (currentAmmo == 0) { Reload(); }
        return false;
    }

    #region Reload
    /// <summary>
    /// Try to initiate a realod
    /// </summary>
    public virtual bool Reload()
    {
        //Check if we can reload
        if (currentAmmo == magSize || totalAmmo == 0 || reloading == true || waitForReload == true)
        {
            return false;
        }

        //Start Reload
        StartCoroutine(ReloadRoutine());
        return true;
    }
    /// <summary>
    /// Play the reload animation and sound
    /// </summary>
    public IEnumerator ReloadRoutine()
    {
        //Reload Delay
        waitForReload = true;
        yield return new WaitForSeconds(waitBeforeReload);
        waitForReload = false;

        //Before Wait
        reloading = true;
        PlaySound(EffectType.reload);
        PlayAnimation(EffectType.reload, "Reload", true);

        //Wait
        yield return new WaitForSeconds(reloadTime);

        FinishedReload();
    }
    /// <summary>
    /// When the reload is finished
    /// </summary>
    public virtual bool FinishedReload()
    {
        //Add Ammo to the mag
        int ammoToAdd = magSize - currentAmmo;

        if (totalAmmo >= ammoToAdd)
        {
            currentAmmo = magSize;
            totalAmmo -= ammoToAdd;
        }
        else
        {
            currentAmmo += currentAmmo;
            currentAmmo = 0;
        }

        //Stop Effects
        PlayAnimation(EffectType.reload, "Reload", false);

        StopCoroutine(ReloadRoutine());
        reloading = false;
        return true;
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
            if (animation.effectType == type && animation.animator != null)
            {
                animation.animator.SetBool(boolname, animationBool);
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
            if (EffectType.fire == type)
            {
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

        if (type != "reload") { soundFX.transform.SetParent(antiHierarchySpam.transform); }
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
    }
    #endregion

    #region Raycast hit logic
    public void HitDetection(RaycastHit hit, Ray weaponRay, int damage)
    {
        //if(hit.transform.gameObject.tag == "Enemy Head" || hit.transform.tag == "Enemy Torso" || hit.transform.tag == "Enemy Left Arm" || hit.transform.tag == "Enemy Right Arm")
        //{
        //    Debug.Log("KYS");
        //    hit.transform.SendMessage("PartDetected");
        //}
        if (hit.transform.CompareTag("Grenade"))
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
    #endregion

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
    }
    public void StopAllWeaponCorutines()
    {
        StopAllCoroutines();
    }
}