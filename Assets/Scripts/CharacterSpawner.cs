using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnPosition
{
    public Transform Transform;
    public Character Owner;

    public Vector3 Position => Transform.position;
    public bool IsOccupied => Owner != null;

    public void AssignCharacter(Character _character)
    {
        _character.transform.position = Position;
        Owner = _character;
    }
}

//TODO: get data from map and spawn enemies
public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnPosition> enemiesPos;
    [SerializeField] private SpawnPosition playerPos;

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

        GetSpawnPosition().AssignCharacter(_newEnemy);
        
        return _newEnemy;
    }

    public Character SpawnEnemy(EnemyDataModifier _enemyDataModifier)
    {
        var _newEnemy = CharacterFactory.CreateCharacterObject(_enemyDataModifier.baseData.name);
        var _hpMod = Random.Range(_enemyDataModifier.minMaxHpMod.x, _enemyDataModifier.minMaxHpMod.y);
        var _startHp = _enemyDataModifier.startHp > 0
            ? _enemyDataModifier.startHp : _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.MAX_HEALTH).Value;
        var _hp = _startHp + _hpMod;
        
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.MAX_HEALTH).Value = _hp;
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.HEALTH).Value = _hp;
        _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.SHIELD).Value = _enemyDataModifier.startShield;
        if (_enemyDataModifier.startSize > 0)
        {
            _newEnemy.runtimeCharacter.properties.Get<int>(PropertyKey.SIZE).Value = _enemyDataModifier.startSize;
        }
            
        _newEnemy.gameObject.tag = "ENEMY";
        
        GetSpawnPosition().AssignCharacter(_newEnemy);

        return _newEnemy;
    }

    public Character SpawnPlayer(string _name)
    {
        var _player = CharacterFactory.CreateCharacterObject(_name);
        _player.gameObject.tag = "PLAYER";
        
        playerPos.AssignCharacter(_player);
        
        return _player;
    }

    private SpawnPosition GetSpawnPosition()
    {
        foreach (var _position in enemiesPos)
        {
            if (_position.IsOccupied) continue;
            
            return _position;
        }
        
        Debug.Log("TOO MANY ENEMIES");

        return enemiesPos[0];
    }
}
