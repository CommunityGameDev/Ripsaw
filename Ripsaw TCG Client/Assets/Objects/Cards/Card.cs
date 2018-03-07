using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    public long _id;
    public string _name;
    public string _description;
    public long? _attack;
    public long? _hp;
    public Dictionary<Resource, long> _resourcesToFlip = new Dictionary<Resource, long>();
    public Dictionary<Resource, long> _resourcesToPlay = new Dictionary<Resource, long>();
    public Pack _pack = new Pack();
    public Set _set = new Set();
    

    public Card(long id, string name, string description, long? attack, long? hp, Dictionary<Resource, long> resourcesToFlip, Dictionary<Resource, long> resourcesToPlay, Pack pack, Set set)
    {
        _id = id;
        _name = name;
        _description = description;
        _attack = attack;
        _hp = hp;
        _resourcesToFlip = resourcesToFlip;
        _resourcesToPlay = resourcesToPlay;
        _pack = pack;
        _set = set;
    }
	


}
