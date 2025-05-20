#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects] //Allows editing multiple objects at once
[CustomEditor(typeof(Weapon), true)] //The `true` parameter makes it work for derived 
public class WeaponTypeEditor : Editor
{
    #region Variables
    private bool[] fireEffectsFoldouts;

    //Serialized properties
    SerializedProperty needsAmmo;
    SerializedProperty currentAmmo;
    SerializedProperty totalAmmo;
    SerializedProperty magSize;
    SerializedProperty maxAmmo;
    SerializedProperty ammoText;
    SerializedProperty waitBeforeReload;
    SerializedProperty reloadTime;
    SerializedProperty chanceToPlayReloadB;
    SerializedProperty firedelay;
    SerializedProperty pullOutDelay;
    SerializedProperty switchDelay;
    SerializedProperty LayerMask;
    SerializedProperty weaponRange;
    SerializedProperty flashLight;
    SerializedProperty antiHierarchySpam;
    SerializedProperty screenShakeDuration;
    SerializedProperty screenShakeIntensity;
    #endregion

    #region OnEnable
    private void OnEnable()
    {
        currentAmmo = serializedObject.FindProperty("currentAmmo");
        totalAmmo = serializedObject.FindProperty("totalAmmo");
        magSize = serializedObject.FindProperty("magSize");
        maxAmmo = serializedObject.FindProperty("maxAmmo");
        ammoText = serializedObject.FindProperty("ammoText");
        waitBeforeReload = serializedObject.FindProperty("waitBeforeReload");
        reloadTime = serializedObject.FindProperty("reloadTime");
        chanceToPlayReloadB = serializedObject.FindProperty("chanceToPlayReloadB");
        firedelay = serializedObject.FindProperty("firedelay");
        switchDelay = serializedObject.FindProperty("pullOutDelay");
        pullOutDelay = serializedObject.FindProperty("switchDelay");
        LayerMask = serializedObject.FindProperty("hitMask");
        weaponRange = serializedObject.FindProperty("weaponRange");
        screenShakeDuration = serializedObject.FindProperty("screenShakeDuration");
        screenShakeIntensity = serializedObject.FindProperty("screenShakeIntensity");
        flashLight = serializedObject.FindProperty("flashLight");
        antiHierarchySpam = serializedObject.FindProperty("antiHierarchySpam");
        needsAmmo = serializedObject.FindProperty("needsAmmo");
    }
    #endregion

    #region DrawLine
    private void DrawLine(Color color, int thickness = 1, int padding = 10)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, thickness + padding);
        rect.height = thickness;
        rect.y += padding / 2;
        EditorGUI.DrawRect(rect, color);
    }
    #endregion

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Script reference
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Weapon)target), typeof(Weapon), false);
        EditorGUI.EndDisabledGroup();

        //ERROR message when using Weapon.cs insted of a derived class
        if (target.GetType() == typeof(Weapon))
        {
            EditorGUILayout.HelpBox("Editing the base Weapon class directly is not supported. Please use a derived class.", MessageType.Error);
            return;
        }

        //OBS message
        GUIStyle OBSHeader = new(EditorStyles.boldLabel) { richText = true };
        EditorGUILayout.LabelField("OBS: <color=yellow>Variables</color> for each Weapon class are at the <color=yellow>bottom</color> under <color=#03fce3>Effects</color>", OBSHeader);
        EditorGUILayout.Space();

        //Needs Ammo toggle with color
        GUIStyle toggleStyle = new(EditorStyles.label) { richText = true };
        string labelColor = needsAmmo.boolValue ? "green" : "red";
        needsAmmo.boolValue = EditorGUILayout.ToggleLeft($"<b><color={labelColor}>Requires Ammo?</color></b>", needsAmmo.boolValue, toggleStyle);

        DrawLine(Color.white, 2, 5);

        //Ammo & Reload section
        if (needsAmmo.boolValue)
        {
            EditorGUILayout.PropertyField(currentAmmo);
            EditorGUILayout.PropertyField(totalAmmo);
            EditorGUILayout.PropertyField(magSize);
            EditorGUILayout.PropertyField(maxAmmo);
            EditorGUILayout.PropertyField(ammoText);
            EditorGUILayout.PropertyField(waitBeforeReload);
            EditorGUILayout.PropertyField(reloadTime);
            EditorGUILayout.PropertyField(chanceToPlayReloadB);
            DrawLine(Color.white, 2, 5);
        }

        //Always show these properties
        EditorGUILayout.PropertyField(firedelay);
        EditorGUILayout.PropertyField(pullOutDelay);
        EditorGUILayout.PropertyField(switchDelay);

        EditorGUILayout.Space();

        GUIStyle CollisonHeader = new(EditorStyles.boldLabel) { richText = true };
        EditorGUILayout.LabelField("Collision", CollisonHeader);
        Color originalColor = GUI.contentColor;
        GUI.contentColor = LayerMask.intValue == 0 ? Color.red : Color.white;
        EditorGUILayout.PropertyField(LayerMask, new GUIContent("Hit Mask"));
        GUI.contentColor = originalColor;

        if (LayerMask.intValue == 0)
        {
            EditorGUILayout.HelpBox("You need to add a layer otherwise no raycast will register", MessageType.Info);
        }

        EditorGUILayout.PropertyField(weaponRange);
        EditorGUILayout.PropertyField(screenShakeDuration);
        EditorGUILayout.PropertyField(screenShakeIntensity);
        EditorGUILayout.PropertyField(flashLight);
        EditorGUILayout.PropertyField(antiHierarchySpam);

        DrawLine(Color.white, 2, 5);

        //Effects array
        SerializedProperty effectsProperty = serializedObject.FindProperty("effects");
        if (fireEffectsFoldouts == null || fireEffectsFoldouts.Length != effectsProperty.arraySize)
        {
            fireEffectsFoldouts = new bool[effectsProperty.arraySize];
            for (int i = 0; i < fireEffectsFoldouts.Length; i++)
                fireEffectsFoldouts[i] = true;
        }

        for (int i = 0; i < effectsProperty.arraySize; i++)
        {
            SerializedProperty effect = effectsProperty.GetArrayElementAtIndex(i);
            SerializedProperty effectType = effect.FindPropertyRelative("effectType");
            SerializedProperty animators = effect.FindPropertyRelative("animators");
            SerializedProperty audioClip = effect.FindPropertyRelative("audioClip");
            SerializedProperty pitch = effect.FindPropertyRelative("pitch");
            SerializedProperty fireEffects = effect.FindPropertyRelative("fireEffects");

            //Effect header
            GUIStyle effectLabelStyle = new(EditorStyles.boldLabel) { richText = true };
            EditorGUILayout.LabelField($"<color=#03fce3>Effect {i + 1}</color>", effectLabelStyle);
            EditorGUILayout.PropertyField(effectType);
            EditorGUILayout.PropertyField(animators, true);
            EditorGUILayout.PropertyField(audioClip);
            EditorGUILayout.PropertyField(pitch);

            //Fire Effects foldout
            if (effectType.enumValueIndex == 0) //Fire
            {
                GUIStyle fireEffectsStyle = new(EditorStyles.foldout) { richText = true };
                fireEffectsFoldouts[i] = EditorGUILayout.Foldout(fireEffectsFoldouts[i], $"<color=#ff4800>Fire Effects</color>", true, fireEffectsStyle);

                if (fireEffectsFoldouts[i])
                {
                    //"Muzzle Flash" header in yellow
                    GUIStyle yellowHeaderStyle = new(EditorStyles.boldLabel) { richText = true };
                    EditorGUILayout.LabelField("<color=#ffffff>Muzzle Flash</color>", yellowHeaderStyle);

                    //Child properties
                    SerializedProperty muzzleLight = fireEffects.FindPropertyRelative("muzzleLight");
                    SerializedProperty muzzleFlashParticle = fireEffects.FindPropertyRelative("muzzleFlashParticle");
                    SerializedProperty muzzleTime = fireEffects.FindPropertyRelative("muzzleTime");
                    SerializedProperty casing = fireEffects.FindPropertyRelative("casing");
                    SerializedProperty enemyHitParticle = fireEffects.FindPropertyRelative("enemyHitParticle");
                    SerializedProperty enviormentHitParticle = fireEffects.FindPropertyRelative("enviormentHitParticle");

                    EditorGUILayout.PropertyField(muzzleLight, new GUIContent("Muzzle Light"));
                    EditorGUILayout.PropertyField(muzzleFlashParticle, new GUIContent("Muzzle Flash Particle"));
                    EditorGUILayout.PropertyField(muzzleTime);

                    EditorGUILayout.PropertyField(enemyHitParticle);
                    EditorGUILayout.PropertyField(enviormentHitParticle);
                    EditorGUILayout.PropertyField(casing);
                }
            }

            DrawLine(Color.gray, 1, 5);
        }

        //Add/Remove Effect buttons
        if (effectsProperty.arraySize < System.Enum.GetValues(typeof(EffectType)).Length)
        {
            if (GUILayout.Button("Add Effect"))
            {
                effectsProperty.arraySize++;
                SerializedProperty newEffect = effectsProperty.GetArrayElementAtIndex(effectsProperty.arraySize - 1);

                //Set default pitch to (1,1)
                SerializedProperty pitch = newEffect.FindPropertyRelative("pitch");
                pitch.vector2Value = new Vector2(1, 1);

                //Assign next available EffectType
                SerializedProperty newEffectType = newEffect.FindPropertyRelative("effectType");
                newEffectType.enumValueIndex = effectsProperty.arraySize - 1;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("All EffectTypes have been added.", MessageType.Info);
        }

        if (effectsProperty.arraySize > 0 && GUILayout.Button("Remove Last Effect"))
        {
            effectsProperty.arraySize--;
        }

        DrawLine(Color.white, 2, 5);
        EditorGUILayout.Space();

        //Header
        GUIStyle RR = new(EditorStyles.boldLabel) { richText = true };
        EditorGUILayout.LabelField("<color=magenta> Weapon Variabels", RR);

        //Draw all remaining properties (including those from derived classes) that haven't been explicitly drawn
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren))
        {
            if (property.name == "m_Script" ||
                property.name == "currentAmmo" ||
                property.name == "totalAmmo" ||
                property.name == "magSize" ||
                property.name == "maxAmmo" ||
                property.name == "ammoText" ||
                property.name == "waitBeforeReload" ||
                property.name == "reloadTime" ||
                property.name == "chanceToPlayReloadB" ||
                property.name == "firedelay" ||
                property.name == "pullOutDelay" ||
                property.name == "switchDelay" ||
                property.name == "hitMask" ||
                property.name == "weaponRange" ||
                property.name == "screenShakeDuration" ||
                property.name == "screenShakeIntensity" ||
                property.name == "flashLight" ||
                property.name == "antiHierarchySpam" ||
                property.name == "needsAmmo" ||
                property.name == "effects")
            {
                enterChildren = false;
                continue;
            }
            EditorGUILayout.PropertyField(property, true);
            enterChildren = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
