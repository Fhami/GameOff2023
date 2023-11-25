using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using AYellowpaper.SerializedCollections;

//public enum Size { Small, Medium, Big }

public class ActiveSkillUI : MonoBehaviour
{
    [SerializeField] List<ActiveSkillIcon> _activeSkill = new List<ActiveSkillIcon>();
    [SerializeField] SerializedDictionary<CardData, ActiveSkillIcon> _skills;

    private void Start()
    {
        // foreach(var icon in _activeSkill)
        // {
        //     icon.gameObject.SetActive(false);
        // }
    }

    public void SetSkill(int slot, ActiveSkillDetail  skillDetail, Action onClick)
    {
        _activeSkill[slot].SetSkill(skillDetail.CardData, onClick);
        _activeSkill[slot].SetSize(skillDetail.Size);
        _activeSkill[slot].SetSizeNumber(skillDetail.SizeNumber);
        _activeSkill[slot].gameObject.SetActive(true);

        if (!_skills.TryGetValue(skillDetail.CardData, out var icon))
        {
            _skills.Add(skillDetail.CardData, _activeSkill[slot]);
        }
        else
        {
            _skills[skillDetail.CardData] = _activeSkill[slot];
        }


    }

    public void EnableSkill(CardData cardData , bool enable)
    {
        _skills[cardData].SetEnable(enable);
    }

    public void RemoveSkill(int slot)
    {
        _activeSkill[slot].gameObject.SetActive(false);
    }

    public void AddSkill(int slot)
    {
        _activeSkill[slot].gameObject.SetActive(true);
    }

}

public class ActiveSkillDetail
{
    private CardData cardData;
    private Size size;
    private int sizeNumber;

    public CardData CardData { get => cardData; set => cardData = value; }
    public Size Size { get => size; set => size = value; }
    public int SizeNumber { get => sizeNumber; set => sizeNumber = value; }

    public ActiveSkillDetail(CardData cardData, Size size)
    {
        this.CardData = cardData;
        this.Size = size;
    }

    public ActiveSkillDetail(CardData cardData,int sizeNumber, Size size)
    {
        this.CardData = cardData;
        this.Size = size;
        this.SizeNumber = sizeNumber;
    }
}
