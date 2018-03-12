using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    public long ID;
    public string _name;
    public string _description;
    public long? _attack;
    public long? _hp;
    public Dictionary<Resource, long> _resourcesToFlip = new Dictionary<Resource, long>();
    public Dictionary<Resource, long> _resourcesToPlay = new Dictionary<Resource, long>();
    public Pack _pack = new Pack();
    public Set _set = new Set();
    

    
	


}
