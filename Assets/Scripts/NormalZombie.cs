using UnityEngine;
using System.Collections;

public class NormalZombie : ZombieBase {

    [SerializeField] int newHealth;

	protected override void Start()
	{
		base.Start();
		damage = 10;
		health = newHealth;
		HealthBar.maxValue = health;
		HealthBar.value = health;
	}
	

}
