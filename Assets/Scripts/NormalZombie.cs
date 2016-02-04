using UnityEngine;
using System.Collections;

public class NormalZombie : ZombieBase {

	protected override void Start()
	{
		base.Start();
		damage = 10;
		health = 100;
		HealthBar.maxValue = health;
		HealthBar.value = health;
	}
	

}
