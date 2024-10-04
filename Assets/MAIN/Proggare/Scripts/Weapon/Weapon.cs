using UnityEngine;
using System.Collections;
using TMPro;

//Sebbe

public enum EffectType
{
    fire = 0,
    reload = 1,
    pullout = 2,
    equip = 3,
}

public class Weapon : MonoBehaviour
{
    //Configurable Perameters
    [Header("Ammo")]
    public int currentAmmo;
    public int totalAmmo = 90;
    public int magSize = 30;
    public TextMeshProUGUI ammoText;

    [Header("Delays")]
    public float reloadTime = 3f;
    public float firedelay = 3f;

    [Header("Extra")]
    [Tooltip("Instansiate Gameobjects on transform for a clearer Hierarchy")] public GameObject antiHierarchySpam;
    public LayerMask hitMask = 0;
    public float weaponRange = 100f;
    protected Camera mainCam = null;

    [Header("Effects")]
    public Effects[] effects;

    [System.Serializable]
    public struct Effects
    {
        public EffectType effectType;
        public AudioClip audioClip;
        public Animator animator;
        public ParticleSystem hitParticle;
    }

    //Private Variabels
    private float currentFireDelay;
    private bool reloading = false;

    #region Base Methods
    protected void Start()
    {
        //Get Camera
        mainCam = Camera.main;

        //Set currentAmmo in start
        if (currentAmmo == 0)
        {
            currentAmmo = magSize;
        }
    }
    private void Update()
    {
        //Update the ammo UI when the gun is enabled
        if (gameObject.activeSelf && ammoText != null) { ammoText.text = currentAmmo.ToString() + "/" + totalAmmo.ToString(); }


        //Dont Spam the Hierarchy
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }

        //Cooldowns
        if (currentFireDelay > 0) { currentFireDelay -= Time.deltaTime; }
        else { currentFireDelay = 0; PlayAnimation(EffectType.fire, "Fire", false); }
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
        if(currentAmmo > 0 && currentFireDelay == 0 && reloading == false)
        {
            //Logic
            currentAmmo--;
            currentFireDelay = firedelay;
            if (currentAmmo == 0) { Reload(); }

            //Effects
            PlayAnimation(EffectType.fire, "Fire", true);
            PlaySound(EffectType.fire);

            return true;
        }

        //false
        if (currentAmmo == 0) { Reload(); }
        return false;
    }
    public virtual bool Reload()
    {
        //Reload
        if (currentAmmo == magSize || totalAmmo == 0 || reloading == true)
        {
            return false;
        }

        //Start Reload
        StartCoroutine(ReloadRoutine());
        return true;
    }
    public IEnumerator ReloadRoutine()
    {
        //Before Wait
        reloading = true;
        PlaySound(EffectType.reload);
        PlayAnimation(EffectType.reload, "Reload", true);

        //wait
        yield return new WaitForSeconds(reloadTime);
        
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

    public void PlayEffects(EffectType type, string boolname, bool effectBool)
    {
        foreach (var effect in effects)
        {
            if (effect.effectType == type)
            {
                //HIT PARTICEL GET FROM MAIN INSTED OF EVERY WEAPON SCRIPT
            }
        }
    }
    #endregion

    #region Audio Management
    /// <summary>
    /// Play the Audio that is of same <paramref name="type"/>
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
        soundFX.AddComponent<Destroy>();

        //Edit Components
        soundFX.GetComponent<AudioSource>().playOnAwake = false;
        soundFX.GetComponent<AudioSource>().clip = clip;
        soundFX.GetComponent<Destroy>().SetAliveTime(clip.length);

        //Destroy
        soundFX.GetComponent<AudioSource>().Play();
    }
    #endregion
}