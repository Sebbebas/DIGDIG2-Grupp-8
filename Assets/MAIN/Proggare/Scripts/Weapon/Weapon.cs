using UnityEngine;
using System.Collections;
using TMPro;

//Seb Neb
public enum EffectType
{
    fire = 0,
    reload = 1,
    pullout = 2,
    equip = 3,
}

public class Weapon : MonoBehaviour
{
    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
    }

    //Configurable Perameters
    [Header("Ammo")]
    public int currentAmmo;
    public int totalAmmo = 90;
    public int magSize = 30;
    public int maxAmmo = 80;
    public TextMeshProUGUI ammoText;
    public GameObject shellOne, shellTwo;

    [Header("Delays")]
    public float waitBeforeReload = 0.5f;
    public float reloadTime = 3f;
    public float firedelay = 3f;

    [Header("Extra")]
    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] public GameObject antiHierarchySpam;
    public LayerMask hitMask = 0;
    public float weaponRange = 100f;
    protected Camera mainCam = null;

    [Header("Effects")]
    [Tooltip("When calling a action play the effects with the same EffectType")]public Effects[] effects;

    [Header("Screen Shake")]
    [SerializeField] float screenShakeDuration = 0.1f;
    [SerializeField] float screenShakeIntensity = 0.1f;

    [System.Serializable]
    public struct Effects
    {
        [Header("Audio & Animation Effects")]
        public EffectType effectType;
        public AudioClip audioClip;
        public Animator animator;

        public FireEffects fireEffects;

        [System.Serializable]
        public struct FireEffects
        {
            [Header("Muzzle Flash")]
            [Tooltip("Reference for enabling the muzzleFlash")] public Light muzzleLight;
            [Tooltip("Reference for random muzzleFlash particle rotation")] public ParticleSystem muzzleFlashParticle;
            [Tooltip("MAN FIGURE IT OUT")] public float muzzleTime;

            [Space]

            public Light flashLight;
            public GameObject casing;
            public ParticleSystem hitParticle;
        }
    }

    //Private Variabels
    private float currentFireDelay;
    private bool reloading = false;
    private bool waitForReload = false;

    //Chaced References
    ScreenShake screenShake;

    #region Base Methods
    protected void Start()
    {
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
        if (gameObject.activeSelf && ammoText != null) { ammoText.text = totalAmmo.ToString(); }

        //REMOVE THIS WHEN YOU CAN FIND AND PICKUP WEAPON\\
        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Cooldowns
        if (currentFireDelay > 0) { currentFireDelay -= Time.deltaTime; }
        else { currentFireDelay = 0; PlayAnimation(EffectType.fire, "Fire", false); }

        //Ammo won't go over a certain limit
        if (totalAmmo > maxAmmo) { totalAmmo = maxAmmo; }
    }
    private void OnEnable()
    {
        // When the weapon is Enabled \\

        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Start reload
        if (reloading) { StartCoroutine(ReloadRoutine()); }
    }
    #endregion

    #region Virtual Bools
    public virtual bool Fire()
    {
        //true
        if(currentAmmo > 0 && currentFireDelay == 0 && reloading == false && waitForReload == false)
        {
            //Screen Shake
            screenShake.Shake(screenShakeDuration, screenShakeIntensity);

            //Logic
            currentAmmo--;
            currentFireDelay = firedelay;
            if (currentAmmo == 0) { Reload(); }

            if (currentAmmo == 2)
            {
                shellOne.SetActive(true);
                shellTwo.SetActive(true);
            }
            else if (currentAmmo == 1)
            {
                shellOne.SetActive(false);
                shellTwo.SetActive(true);
            }
            else if (currentAmmo == 0)
            {
                shellOne.SetActive(false);
                shellTwo.SetActive(false);
            }

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
    public virtual bool Reload()
    {
        //Reload
        if (currentAmmo == magSize || totalAmmo == 0 || reloading == true || waitForReload == true)
        {
            return false;
        }

        //Start Reload
        StartCoroutine(ReloadRoutine());
        return true;
    }
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

        //wait
        yield return new WaitForSeconds(reloadTime);

        //GUI
        shellOne.SetActive(true);
        shellTwo.SetActive(true);

        //After Wait
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

        reloading = false;
        PlayAnimation(EffectType.reload, "Reload", false);
        StopCoroutine(ReloadRoutine());
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
            //Muzzle Flash
            if (EffectType.fire == type)
            {
                //Random rotation of the muzzleFlash particle
                if (effect.fireEffects.muzzleFlashParticle != null) 
                {
                    effect.fireEffects.muzzleFlashParticle.startRotation = Random.Range(0f, 365f);
                }

                //Turn on the muzzle flash for set time
                effect.fireEffects.muzzleLight?.transform.gameObject.SetActive(true);
                yield return new WaitForSeconds(effect.fireEffects.muzzleTime);
                effect.fireEffects.muzzleLight?.transform.gameObject.SetActive(false);

                StopCoroutine(EffectsCoroutine(EffectType.fire));
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
            if(audio.effectType == type && audio.audioClip != null)
            {
                PlaySoundEffect(audio.audioClip);
            }
        }
    }
    public void PlaySoundEffect(AudioClip clip)
    {
        //Create New Object
        GameObject soundFX = new("SoundEffect");

        //Set Transform
        soundFX.transform.position = Camera.main.transform.position;
        soundFX.transform.SetParent(antiHierarchySpam.transform);

        //Add Components
        soundFX.AddComponent<AudioSource>();
        soundFX.AddComponent<AudioPauseManager>();
        soundFX.AddComponent<Destroy>();

        //Edit Components
        soundFX.GetComponent<AudioSource>().playOnAwake = false;
        soundFX.GetComponent<AudioSource>().clip = clip;
        soundFX.GetComponent<Destroy>().SetAliveTime(clip.length);

        //Destroy
        soundFX.GetComponent<AudioSource>().Play();
    }
    #endregion

    #region Get
    public bool GetReloading()
    {
        return reloading;
    }
    #endregion
}