namespace Zuul;

public class Enemy
{
	private Player player { get;  }
	public bool encounterStart = false;
	private int _health = 100;
	private int maxHealth = 100;

	public int currentHealth()
	{
		return _health;
	}

	public int takeDamage(int taken = 0)
	{
		//Check Used item like ''atack enemy sword''. cant run away when the enemy appears; 
		_health = _health - taken;
		if (_health > 0)
		{
			Console.WriteLine("Enemy health:");
			Console.WriteLine(currentHealth());
		} else if (_health <= 0)
		{
			Console.WriteLine("There is no enemy alive");
		}
		Console.WriteLine(isAlive());
		return _health;
	}

	private bool Alive;

	public bool isAlive()
	{
			if (_health > 0)
			{
				Alive = true;
				return Alive;
			}
			else
			{
				Console.WriteLine("Enemy Defeated!");
				Alive = false;
				return Alive;
			}
	}
}
