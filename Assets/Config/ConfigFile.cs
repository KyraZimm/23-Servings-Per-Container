using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Scriptable Objects/Config")]
public class ConfigFile : ScriptableObject
{
    public Config Settings;
    public static implicit operator Config(ConfigFile file) => file.Settings;
}
