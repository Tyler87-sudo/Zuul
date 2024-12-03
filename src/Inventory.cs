using System.Dynamic;
using System.Security.Cryptography;

namespace Zuul;

public class Inventory
{
	private int maxWeight;

	private Dictionary<string, Item> items;

	public Inventory(int maxWeight)
	{
		this.maxWeight = maxWeight;
		this.items = new Dictionary<string, Item>();
	}
	
	public bool Put(string itemName, Item item)
	{
			items.Add(itemName, item);
			return true;
	}

	//public int ItemCount;

	public int countItems()
	{
		return items.Count;
	}

	public void Show()
	{
		foreach (KeyValuePair<string, Item> entry in items)
		{
			Item item = items[entry.Key]; 
			Console.WriteLine(entry.Key + ", item weight is: " + item.Weight + "kg");
		}
	}

	public Item Get(string itemName)
	{
		Console.WriteLine(itemName);
		if (items.ContainsKey(itemName))
		{
			Item item = items[itemName];
			items.Remove(itemName);
			return item;
		}
		else
		{
			Console.WriteLine("You can't take or deposit an item you do not have or is not in the room");
			return null;
		}
	}

	private int TotalWeight()
	{
		int total = 0;
		foreach (string itemName in items.Keys)
		{
			total += items[itemName].Weight;
		}
		return total;
	}

	public int FreeWeight()
	{
		return maxWeight - TotalWeight();
	}

	public int Power(string itemName)
	{
		if (items.ContainsKey(itemName))
		{
			Item item = items[itemName];
			return item.Power;
		}
		else
		{
			Console.WriteLine("You don't have this weapon");
			return 0;
		}
		
	}

	public bool Exists(string itemName)
	{
		return items.ContainsKey(itemName);
	}

//Make a list/array/dictionary of items, and use math.random to pick a random item
	//each movement between rooms using the rnd generator

}

