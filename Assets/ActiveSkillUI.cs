using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using AYellowpaper.SerializedCollections;

public enum Size { Small, Medium, Big }

public class ActiveSkillUI : MonoBehaviour
{
    [SerializeField] List<ActiveSkillIcon> _activeSkill = new List<ActiveSkillIcon>();
    [SerializeField] SerializedDictionary<EffectData, ActiveSkillIcon> _skills;

    private void Start()
    {
        foreach(var icon in _activeSkill)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public void SetSkill(int slot, ActiveSkillDetail  skillDetail, Action onClick)
    {
        _activeSkill[slot].SetSkill(skillDetail.EffectData, onClick);
        _activeSkill[slot].SetSize(skillDetail.Size);
        _activeSkill[slot].gameObject.SetActive(true);

        if (!_skills.TryGetValue(skillDetail.EffectData, out var icon))
        {
            _skills.Add(skillDetail.EffectData, _activeSkill[slot]);
        }
        else
        {
            _skills[skillDetail.EffectData] = _activeSkill[slot];
        }
    }

    public void EnableSkill(EffectData effectData , bool enable)
    {
        _skills[effectData].SetEnable(enable);
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
    private EffectData effectData;
    private Size size;

    public EffectData EffectData { get => effectData; set => effectData = value; }
    public Size Size { get => size; set => size = value; }

    public ActiveSkillDetail(EffectData effectData, Size size)
    {
        this.EffectData = effectData;
        this.Size = size;
    }
}
