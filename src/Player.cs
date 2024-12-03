namespace Zuul;


class Player
{
	public Inventory backpack;
	private Enemy enemy = new Enemy();

	public string status;

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
		if (dealt > _health)
		{
			dealt = _health;
		}
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

	public bool isAlive()
	{
		if (_health > 0)
		{
			Console.WriteLine("Wow! Still alive! Good job!");
			return true;
		}
		else
		{
			Console.WriteLine("I Think you might be dead...");
			return false;
		}
	}

	public void StatusEffect(string status)
	{
		this.status = status;
		if (status == "Poisoned")
		{
			Console.WriteLine("You have been poisoned");
		} else if (status == "Disgusted")
		{
			Console.WriteLine("You are disgusted");
		} else if (status == "Sick")
		{
			Console.WriteLine("You are sick, you have puked your organs out");
			Damage(100); 
		}
	}

	public void Use(string itemName)
	{
		if (itemName == "potion")
		{
			backpack.Get(itemName);
			Heal(50);
			Console.WriteLine("Your health now");
			Console.WriteLine(currentHealth());
		}
		else
		{
			Console.WriteLine("You can't use this item");
		} ;
	}

	public Player()
	{
		_health = 100;
		CurrentRoom = null;
		backpack = new Inventory(60);
	}

	public bool TakeFromChest(string itemName)
	{
		Item currentitem = CurrentRoom.Chest.Get(itemName);
		if (currentitem == null)
		{
			return false;
		}
		if (currentitem.Description == "The golden hat")
		{
			Console.WriteLine("You picked up the golden hat, and put it on your head. However, you underestimated it's weight. You were crushed! Glorious victory!");
			_health = 0;
			isAlive();
			return true;
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
		Item currentItem = backpack.Get(itemName);
		if (currentItem == null)
		{
			return false;
		}
		CurrentRoom.Chest.Put(itemName, currentItem);
		Console.WriteLine("items currently in room chest:");
		CurrentRoom.Chest.Show();
		return true;
	}

	public void Attack(string item = null)
	{
		int attackPower = backpack.Power(itemName: item);
		if (attackPower == 0)
		{
			Console.WriteLine("The enemy has not taken any damage");
			return;
		}
		enemy.takeDamage(attackPower);
	}
	
}