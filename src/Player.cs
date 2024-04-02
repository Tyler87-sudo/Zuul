namespace Zuul;


class Player
{
	public Inventory backpack;
	private Enemy enemy = new Enemy();

	public bool canMove = true;

	public Room CurrentRoom { get; set; }

	private int _health;
	private int maxHealth = 100;

	public int currentHealth()
	{
		return _health;
	}

	public int Damage(int dealt)
	{
		_health = _health - dealt;
		Console.WriteLine("You have taken damage: " + dealt + " health points lost!");
		return _health;
	}

	public int Heal(int amount)
	{
		int _prev_health = _health;
		_health = _health + amount;
		if (_health > maxHealth)
		{
			_health = maxHealth;
			amount = maxHealth - _prev_health;
		}
		Console.WriteLine("You have healed for " + amount + " health points!");
		return _health;
	}

	private bool Alive;

	public bool isAlive()
	{
		if (_health > 0)
		{
			Alive = true;
			Console.WriteLine("Wow! Still alive! Good job!");
			return Alive;
		}
		else
		{
			Alive = false;
			Console.WriteLine("I think you might be dead...");
			return Alive;
		}
	}

	public string Use(string itemName)
	{
		if (itemName == "Potion")
		{
			backpack.Get(itemName);
			Heal(20);
			Console.WriteLine("Your health now");
			Console.WriteLine(currentHealth());
		}
		return "yay";
	}

	public Player()
	{
		_health = 100;
		CurrentRoom = null;
		backpack = new Inventory(100);
	}

	public bool TakeFromChest(string itemName)
	{
		Item currentitem = CurrentRoom.Chest.Get(itemName);
		if (currentitem == null)
		{
			return false;
		}
		if (currentitem.Weight <= backpack.FreeWeight())
		{
			backpack.Put(itemName, currentitem);
			Console.WriteLine("item added {0}", currentitem.Description);
			Console.WriteLine("Items currently in backpack:");
			Console.WriteLine("Current free weight in backpack: {0}", backpack.FreeWeight());
			backpack.Show();
			return true;
		}
		else
		{
			CurrentRoom.Chest.Put(itemName, currentitem);
			Console.WriteLine("weight of the item: " + currentitem.Weight);
			Console.WriteLine("I think your inventory is full, so you can't pick this up");
			return false;
		}
	}

	public bool DropToChest(string itemName)
	{
		CurrentRoom.Chest.Put(itemName, backpack.Get(itemName));
		Console.WriteLine("items currently in room chest:");
		Console.WriteLine(CurrentRoom.Chest.Show());
		return true;
	}

	public void Attack(string item = null)
	{
		int attackPower = backpack.Power(item);
		if (attackPower == 0)
		{
			Console.WriteLine("The enemy has not taken any damage");
			return;
		}
		enemy.takeDamage(attackPower);
	}
	
}