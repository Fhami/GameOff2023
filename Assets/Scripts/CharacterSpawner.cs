using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct SpawnPosition
{
    public Transform Transform;
    public Character Owner;

    public Vector3 Position => Transform.position;
    public bool IsOccupied => Owner != null;
}

//TODO: get data from map and spawn enemies
public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnPosition> enemiesPos;
    [SerializeField] private SpawnPosition playerPos;

    public List<Character> SpawnEnemies(List<string> _names)
    {
        var _newEnemies = new List<Character>();

        for (var _index = 0; _index < _names.Count; _index++)
        {
            var _transform = enemiesPos[_index];
            var _name = _names[_index];
            
            var _enemy = CharacterFactory.CreateCharacterObject(_name);
            _enemy.gameObject.tag = "ENEMY";
            _enemy.transform.position = _transform.Position;

            _newEnemies.Add(_enemy);
        }
        
        return _newEnemies;
    }

    public List<Character> SpawnEnemies(EncounterData _encounterData)
    {
        var _newEnemies = new List<Character>();

        foreach (var _enemyDataModifier in _encounterData.enemies)
        {
            var _newEnemy = SpawnEnemy(_enemyDataModifier);
            
            _newEnemies.Add(_newEnemy);
        }

        return _newEnemies;
    }

    public Character SpawnEnemy(string _name)
    {
        var _newEnemy = CharacterFactory.CreateCharacterObject(_name);
        _newEnemy.gameObject.tag = "ENEMY";

        return _newEnemy;
    }

    public Character SpawnEnemy(EnemyDataModifier _enemyDataModifier)
    {
        var _newEnemy = CharacterFactory.CreateCharacterObject(_enemyDataModifier.baseData.name);
        var _hpMod = Random.Range(_enemyDataModifier.minMaxHpMod.x, _enemyDataModifier.minMaxHpMod.y);
        var _hp = _enemyDataModifier.startHp + _hpMod;
        
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.MAX_HEALTH).Value = _hp;
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).Value = _hp;
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).Value = _enemyDataModifier.startShield;
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).Value = _enemyDataModifier.startSize;
            
        _newEnemy.gameObject.tag = "ENEMY";

        return _newEnemy;
    }

    public Character SpawnPlayer(string _name)
    {
        var _player = CharacterFactory.CreateCharacterObject(_name);
        _player.gameObject.tag = "PLAYER";

        return _player;
    }
}
