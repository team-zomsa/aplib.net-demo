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
    /// Checks the ID of all keys in the keyring against the inputted doorId, if a match is found, true is returned and the key is consumed.
    /// </summary>
    /// <param name="doorId">The ID of the inputted door that is then checked against all the keys in the keyring.</param>
    /// <returns>True if the correct key is present otherwise False.</returns>
    public bool KeyQuery(Door door)
    {
        foreach (Key k in _keyRing)
        {
            bool opened = door.TryOpenDoor(k);
            if (opened)
            {
                _keyRing.Remove(k);
                return true;
            }
        }

        return false;
    }
}
