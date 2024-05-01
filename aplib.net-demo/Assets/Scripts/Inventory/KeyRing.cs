using System.Collections.Generic;
using UnityEngine;

public class KeyRing : MonoBehaviour
{
    private List<Key> _keyRing;

    /// <summary>
    /// Creates the keyring
    /// </summary>
    private void Start() => _keyRing = new List<Key>();

    /// <summary>
    /// Adds a key to the keychain.
    /// </summary>
    /// <param name="key">The key to be stored.</param>
    public void StoreKey(Key key) => _keyRing.Add(key);

    /// <summary>
    /// Checks the ID of all keys in the keyring against the inputted doorId, if a match is found, true is returned and the key is consumed.
    /// </summary>
    /// <param name="doorId">The ID of the inputted door that is then checked against all the keys in the keyring.</param>
    /// <returns></returns>
    public bool KeyQuery(Door door)
    {
        foreach (Key k in _keyRing)
        {
            _ = door.TryOpenDoor(k);
        }

        return false;

    }
}
