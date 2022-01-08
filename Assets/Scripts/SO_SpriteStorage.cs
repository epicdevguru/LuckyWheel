/****************************************************
 *             SO_SpriteStorage.cs
 * - Description: It is the script for the scriptable 
 *  object that stores the possible sprites in the 
 *  project including wheel items etc for dynamic
 *  wheel generation.
 * - Made By: Toshiyuki Hara
 * - CO-OP: 
 *              2022-01-08
 * *************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteStore", menuName = "Lucky Wheel/Sprite Store", order = 3)]
public class SO_SpriteStorage : ScriptableObject
{
    public List<Sprite> listWheelSprites;
}
