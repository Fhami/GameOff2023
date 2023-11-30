using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using NaughtyAttributes;
using UnityEngine;

public class TestReward : MonoBehaviour
{
    public List<CardData> rewardPool;
    public RewardController RewardController;

    [Button()]
    public void GenerateReward()
    {
        if (Application.isPlaying)
        {
            RewardController.GenerateCards(rewardPool);
        }
    }
}
