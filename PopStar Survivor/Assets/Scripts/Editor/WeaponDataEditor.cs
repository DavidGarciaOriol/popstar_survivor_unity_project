using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{

    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    void OnEnable()
    {
        // Almacenamos los valores de los datos del arma.
        weaponData = (WeaponData)target;

        // Obtenemos los subtipos del arma y se almacenan.
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
            .ToList();

        // Controlamos opción None.
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        // Nos aseguramos de estar usando el subtipo correcto de arma.
        selectedWeaponSubtype = Mathf.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        // Dibuja un dropdown en el Inspector.
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);
        
        if (selectedWeaponSubtype > 0)
        {
            // Actualiza el campo del behaviour.
            weaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();

            // Marca el objeto a guardar.
            EditorUtility.SetDirty(weaponData);

            // Dibuja los elementos por defecto del Inspector.
            DrawDefaultInspector();
        }
    }
}
