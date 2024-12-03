namespace Zuul;

public class Enemy
{
	private Player player { get; }
	public bool Dead = false;
	private int _health;
	private int maxHealth = 100;


	public int currentHealth()
	{
		return _health;
	}

	public Enemy(int health = 100)
	{
		this.maxHealth = health;
		_health = maxHealth;
	}
	
	

	public int takeDamage(int taken = 0)
	{
		//Check Used item like ''attack enemy sword''. cant run away when the enemy appears; 
		_health = _health - taken;
		if (_health > 0)
		{
			Console.WriteLine("Enemy health:");
			Console.WriteLine(currentHealth());
		}
		else if (_health <= 0)
		{
			Console.WriteLine("The enemy has died!");
			Dead = true;
		}
		return _health;
	}
}
	