using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

//TODO: get data from map and spawn enemies
public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> enemiesPos;

    public List<Character> SpawnEnemies(List<string> _names)
    {
        var _newEnemies = new List<Character>();

        for (var _index = 0; _index < _names.Count; _index++)
        {
            var _transform = enemiesPos[_index];
            var _name = _names[_index];
            
            var _enemy = CharacterFactory.CreateCharacterObject(_name);
            _enemy.gameObject.tag = "ENEMY";
            _enemy.transform.position = _transform.position;

            _newEnemies.Add(_enemy);
        }
        
        return _newEnemies;
    }
}
