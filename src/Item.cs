namespace Zuul;

public class Item
{
	public int Weight { get; }
	public string Description { get; }
	
	public int Power { get; }

	public Item(int weight, string description, int power = 0)
	{
		Weight = weight;
		Description = description;
		Power = power;
	}
}
