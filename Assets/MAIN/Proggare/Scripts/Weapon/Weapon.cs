using UnityEngine;
using System.Collections;
using TMPro;

public enum AudioType
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
    public ParticleSystem hitParticle;
    public Animator animator;
    public AudioClips[] audioFX;

    [System.Serializable]
    public struct AudioClips
    {
        public AudioType audioType;
        public AudioClip audioClip;
    }

    //Private Variabels
    private float currentFireDelay;
    private bool reloading = false;

    #region Base Methods
    protected void Start()
    {
        mainCam = Camera.main;

        if (currentAmmo == 0)
        {
            currentAmmo = magSize;
        }
    }
    private void Update()
    {
        if (gameObject.activeSelf && ammoText != null) { ammoText.text = currentAmmo.ToString() + "/" + totalAmmo.ToString(); }

        //Cooldowns
        if (currentFireDelay > 0) { currentFireDelay -= Time.deltaTime; }
        else { currentFireDelay = 0; if (animator != null) { animator.SetBool("Fire", false); } }
    }
    private void OnEnable()
    {
        if (antiHierarchySpam == null) { antiHierarchySpam = GameObject.FindGameObjectWithTag("antiHierarchySpam"); }
        if (reloading) { StartCoroutine(ReloadRoutine()); }
    }
    #endregion

    #region Virtual Bools
    public virtual bool Fire()
    {
        //true
        if(currentAmmo > 0 && currentFireDelay == 0 && reloading == false)
        {
            currentAmmo--;
            currentFireDelay = firedelay;
            if (animator != null) { animator.SetBool("Fire", true); }
            if (currentAmmo == 0) { Reload(); }
            PlaySound(AudioType.fire);
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
            Debug.Log("cant reload");
            return false;
        }

        StartCoroutine(ReloadRoutine());
        if (animator != null) { animator.SetBool("Reload", true); }
        return true;
    }
    public IEnumerator ReloadRoutine()
    {
        //Before Wait
        reloading = true;
        PlaySound(AudioType.reload);

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
        StopCoroutine(ReloadRoutine());
    }
    #endregion

    #region Audio Management
    public void PlaySound(AudioType type)
    {
        foreach (var audio in audioFX)
        {
            if(audio.audioType == type)
            {
                PlaySoundEffect(audio.audioClip);
            }
        }
    }
    public void PlaySoundEffect(AudioClip clip)
    {
        //Create New Object
        GameObject soundFX = new GameObject("SoundEffect");

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