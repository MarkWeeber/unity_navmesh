using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PersonList : MonoBehaviour
{
    public List<Person> personList;
    void Start()
    {
        personList = FindObjectsOfType<Person>().ToList();
    }

}
