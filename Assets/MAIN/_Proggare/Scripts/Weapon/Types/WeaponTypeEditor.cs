using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon))]
public class WeaponTypeEditor : Editor
{
    private bool[] fireEffectsFoldouts;

    public enum EffectType
    {
        fire = 0,
        reload = 1,
        pullout = 2,
        equip = 3,
    }

    #region SerializedProperties    

    SerializedProperty needsAmmo;

    SerializedProperty currentAmmo;
    SerializedProperty totalAmmo;
    SerializedProperty magSize;
    SerializedProperty maxAmmo;
    SerializedProperty ammoText;

    SerializedProperty waitBeforeReload;
    SerializedProperty reloadTime;

    SerializedProperty firedelay;
    SerializedProperty switchDelay;

    SerializedProperty LayerMask;
    SerializedProperty weaponRange;

    SerializedProperty flashLight;
    SerializedProperty antiHierarchySpam;

    [Header("Screen Shake")]
    SerializedProperty screenShakeDuration;
    SerializedProperty screenShakeIntensity;

    [Header("<color=#03fce3> Effects")]
    [Tooltip("When calling a action play the effects with the same EffectType")] public Effects[] effects;

    [System.Serializable]
    public struct Effects
    {
        [Header("Audio & Animation Effects")]
        SerializedProperty effectType;
        SerializedProperty animator;

        [Space]

        SerializedProperty audioClip;
        SerializedProperty pitch;

        [Space]

        SerializedProperty fireEffects;

        [System.Serializable]
        public struct FireEffects
        {
            [Header("Muzzle Flash")]
            SerializedProperty muzzleLight;
            SerializedProperty muzzleFlashParticle;
            SerializedProperty muzzleTime;

            [Space]

            SerializedProperty casing;
            SerializedProperty enemyHitParticle;
            SerializedProperty enviormentHitParticle;
        }
    }
    #endregion

    private void OnEnable()
    {
        currentAmmo = serializedObject.FindProperty("currentAmmo");
        totalAmmo = serializedObject.FindProperty("totalAmmo");
        magSize = serializedObject.FindProperty("magSize");
        maxAmmo = serializedObject.FindProperty("maxAmmo");
        ammoText = serializedObject.FindProperty("ammoText");

        waitBeforeReload = serializedObject.FindProperty("waitBeforeReload");
        reloadTime = serializedObject.FindProperty("reloadTime");

        firedelay = serializedObject.FindProperty("firedelay");
        switchDelay = serializedObject.FindProperty("switchDelay");

        LayerMask = serializedObject.FindProperty("hitMask");
        weaponRange = serializedObject.FindProperty("weaponRange");

        screenShakeDuration = serializedObject.FindProperty("screenShakeDuration");
        screenShakeIntensity = serializedObject.FindProperty("screenShakeIntensity");

        flashLight = serializedObject.FindProperty("flashLight");
        antiHierarchySpam = serializedObject.FindProperty("antiHierarchySpam");

        needsAmmo = serializedObject.FindProperty("needsAmmo");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Display the script reference at the top
        EditorGUI.BeginDisabledGroup(true); // Make the script field read-only
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Weapon)target), typeof(Weapon), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        // Custom header with colors
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            richText = true // Enable rich text
        };
        EditorGUILayout.LabelField("OBS: <color=yellow>Variables</color> for each Weapon class are at the <color=yellow>bottom</color> under <color=#03fce3>Effects</color>", headerStyle);

        EditorGUILayout.Space();

        // Custom label for "needsAmmo" with dynamic color
        GUIStyle toggleStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true // Enable rich text
        };

        // Change the label color based on the value of needsAmmo
        string labelColor = needsAmmo.boolValue ? "green" : "red";
        needsAmmo.boolValue = EditorGUILayout.ToggleLeft($"<b><color={labelColor}>Requires Ammo?</color></b>", needsAmmo.boolValue, toggleStyle);

        DrawLine(Color.white, 2, 5);

        // Conditionally show Ammo and Reload sections if "needsAmmo" is true
        if (needsAmmo.boolValue)
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(currentAmmo);
            EditorGUILayout.PropertyField(totalAmmo);
            EditorGUILayout.PropertyField(magSize);
            EditorGUILayout.PropertyField(maxAmmo);
            EditorGUILayout.PropertyField(ammoText);

            EditorGUILayout.PropertyField(waitBeforeReload);
            EditorGUILayout.PropertyField(reloadTime);

            EditorGUILayout.Space();
            DrawLine(Color.white, 2, 5);
        }

        // Always show these properties after Ammo/Reload
        EditorGUILayout.PropertyField(firedelay);
        EditorGUILayout.PropertyField(switchDelay);

        string layermaskLabelcolor = LayerMask.arraySize < 1  ? "green" : "red";
        EditorGUILayout.PropertyField(LayerMask);

        EditorGUILayout.PropertyField(LayerMask);

        EditorGUILayout.PropertyField(weaponRange);

        EditorGUILayout.PropertyField(screenShakeDuration);
        EditorGUILayout.PropertyField(screenShakeIntensity);

        EditorGUILayout.PropertyField(flashLight);
        EditorGUILayout.PropertyField(antiHierarchySpam);

        DrawLine(Color.white, 2, 5);

        // Display the "Effects" array with conditional logic
        SerializedProperty effectsProperty = serializedObject.FindProperty("effects");

        // Initialize foldout states if not already done
        if (fireEffectsFoldouts == null || fireEffectsFoldouts.Length != effectsProperty.arraySize)
        {
            fireEffectsFoldouts = new bool[effectsProperty.arraySize];
            for (int i = 0; i < fireEffectsFoldouts.Length; i++)
            {
                fireEffectsFoldouts[i] = true; // Default to expanded
            }
        }

        for (int i = 0; i < effectsProperty.arraySize; i++)
        {
            SerializedProperty effect = effectsProperty.GetArrayElementAtIndex(i);
            SerializedProperty effectType = effect.FindPropertyRelative("effectType");
            SerializedProperty animator = effect.FindPropertyRelative("animator");
            SerializedProperty audioClip = effect.FindPropertyRelative("audioClip");
            SerializedProperty pitch = effect.FindPropertyRelative("pitch");
            SerializedProperty fireEffects = effect.FindPropertyRelative("fireEffects");

            // Display all properties of the Effects struct
            GUIStyle effectLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                richText = true // Enable rich text
            };
            EditorGUILayout.LabelField($"<color=#03fce3>Effect {i + 1}</color>", effectLabelStyle);
            EditorGUILayout.PropertyField(effectType);
            EditorGUILayout.PropertyField(animator);
            EditorGUILayout.PropertyField(audioClip);
            EditorGUILayout.PropertyField(pitch);

            // Conditionally display fireEffects if effectType == fire (0)
            if (effectType.enumValueIndex == (int)EffectType.fire)
            {
                // Custom style for fireEffects label
                GUIStyle fireEffectsStyle = new GUIStyle(EditorStyles.foldout)
                {
                    richText = true // Enable rich text
                };

                // Foldout for fireEffects with custom color
                fireEffectsFoldouts[i] = EditorGUILayout.Foldout(fireEffectsFoldouts[i], $"<color=#f5426f>Fire Effects</color>", true, fireEffectsStyle);

                if (fireEffectsFoldouts[i])
                {
                    // Manually render child properties of fireEffects
                    SerializedProperty muzzleLight = fireEffects.FindPropertyRelative("muzzleLight");
                    SerializedProperty muzzleFlashParticle = fireEffects.FindPropertyRelative("muzzleFlashParticle");
                    SerializedProperty muzzleTime = fireEffects.FindPropertyRelative("muzzleTime");
                    SerializedProperty flashLight = fireEffects.FindPropertyRelative("flashLight");
                    SerializedProperty casing = fireEffects.FindPropertyRelative("casing");
                    SerializedProperty enemyHitParticle = fireEffects.FindPropertyRelative("enemyHitParticle");
                    SerializedProperty enviormentHitParticle = fireEffects.FindPropertyRelative("enviormentHitParticle");

                    EditorGUILayout.PropertyField(muzzleLight, true);
                    EditorGUILayout.PropertyField(muzzleFlashParticle, true);
                    EditorGUILayout.PropertyField(muzzleTime);
                    EditorGUILayout.PropertyField(casing);
                    EditorGUILayout.PropertyField(enemyHitParticle);
                    EditorGUILayout.PropertyField(enviormentHitParticle);
                }
            }

            DrawLine(Color.gray, 1, 5); // Add a separator between effects
        }

        // Automatically assign the next EffectType when adding a new effect
        if (effectsProperty.arraySize < System.Enum.GetValues(typeof(EffectType)).Length)
        {
            if (GUILayout.Button("Add Effect"))
            {
                effectsProperty.arraySize++;
                SerializedProperty newEffect = effectsProperty.GetArrayElementAtIndex(effectsProperty.arraySize - 1);
                SerializedProperty newEffectType = newEffect.FindPropertyRelative("effectType");

                // Assign the next available EffectType
                newEffectType.enumValueIndex = effectsProperty.arraySize - 1;
            }

            if (effectsProperty.arraySize > 0 && GUILayout.Button("Remove Last Effect"))
            {
                effectsProperty.arraySize--;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("All EffectTypes have been added.", MessageType.Info);

            if (effectsProperty.arraySize > 0 && GUILayout.Button("Remove Last Effect"))
            {
                effectsProperty.arraySize--;
            }
        }

        DrawLine(Color.white, 2, 5);

        serializedObject.ApplyModifiedProperties();
    }
    private void DrawLine(Color color, int thickness = 1, int padding = 10)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, thickness + padding);
        rect.height = thickness;
        rect.y += padding / 2;
        EditorGUI.DrawRect(rect, color);
    }
}
