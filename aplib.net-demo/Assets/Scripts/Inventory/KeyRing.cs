using Assets.Scripts.Doors;
using System.Collections.Generic;
using UnityEngine;

public class KeyRing : MonoBehaviour
{
    private readonly List<Key> _keyRing = new();

    /// <summary>
    /// Adds a key to the keyring.
    /// </summary>
    /// <param name="key">The key to be stored.</param>
    public void StoreKey(Key key) => _keyRing.Add(key);

    /// <summary>
    /// Checks the ID of all keys in the keyring against the inputted doorId, if a match is found, true is returned and the key
    /// is consumed.
    /// </summary>
    /// <param name="door">The door that is checked against all the keys in the keyring.</param>
    /// <returns>True if the correct key is present otherwise False.</returns>
    public bool KeyQuery(Door door)
    {
        Key key = _keyRing.Find(door.TryOpenDoor);

        return _keyRing.Remove(key);
    }
}
