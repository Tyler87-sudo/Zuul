using System;
using System.Security.Cryptography;
using Zuul;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private Enemy testEnemy = new Enemy();

	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)

	private Item sword = new Item(50, "Sword", 25);
	private Item scimitar = new Item(50, "Scimitar", 50);
	private Item potion = new Item(5, "Potion, heals 20 health points", 5);

	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room labupperfloor = new Room("on the upper floor of a computing lab");
		Room office = new Room("in the computing admin office", testEnemy);
		Room officebasement = new Room("the basement of the office");

		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);
		lab.AddExit("up", labupperfloor);

		labupperfloor.AddExit("down", lab);

		office.AddExit("west", lab);
		office.AddExit("down", officebasement);

		officebasement.AddExit("up", office);

		outside.Chest.Put("sword", sword);
		outside.Chest.Put("scimitar", scimitar);
		outside.Chest.Put("potion", potion);

		officebasement.Chest.Put("potion", potion);

		// Start game outside

		player.CurrentRoom = outside;

	}

	//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished)
		{
			if (player.currentHealth() > 0)
			{
				Command command = parser.GetCommand();
				finished = ProcessCommand(command);
			}
			else
			{
				Console.WriteLine("Oh dear, You are really dead now!");
				return;
			}
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	public void Look(Command command)
	{
		if (command.HasSecondWord())
		{
			// You can only look where you are, Improve this so that it only reacts to the available rooms. 
			Console.WriteLine("You can't look outside of your current scope, unless you're a magician?");
			return;
		}
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
		Console.WriteLine("Items in room:");
		player.CurrentRoom.Chest.Show();
	}


	public void Status(Command command)
	{
		Console.WriteLine("This is your current health: " + player.currentHealth());
	}

	// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
	private bool ProcessCommand(Command command)
	{
		bool wantToQuit = false;

		if (command.IsUnknown())
		{
			Console.WriteLine("I don't know what you mean...");
			return wantToQuit; // false
		}

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				GoRoom(command);
				break;
			case "quit":
				wantToQuit = true;
				break;
			case "look":
				Look(command);
				break;
			case "status":
				Status(command);
				break;
			case "take":
				player.TakeFromChest(command.SecondWord);
				break;
			case "deposit":
				player.DropToChest(command.SecondWord);
				break;
			case "showbackpack":
				player.backpack.Show();
				break;
			case "use":
				player.Use(command.SecondWord);
				break;
			case "attack":
				Attack(command.SecondWord);
				break;
		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################

	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine("Each movement you make, causes you to lose a random health value between 1 and 10");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	private void Fight()
	{
		Console.WriteLine("You are in a fight with the enemy, what do you want to do?");
		
		
	}

	public void Attack(string item = null) 
	{
		Enemy currentEnemy = player.CurrentRoom.enemy;

		if (currentEnemy != null)
		{
				int attackPower = player.backpack.Power(item);
				if (attackPower == 0)
				{
					Console.WriteLine("The enemy has not taken any damage");
					return;
				}
				
				Console.WriteLine("You attacked the enemy with" + item);
				currentEnemy.takeDamage(attackPower);
		
				Console.WriteLine("The enemy attacked as well!");
			
				Random rnd = new Random();
				int num = rnd.Next();
				player.Damage(rnd.Next(10) + 1);
				
		}
		else
		{
				Console.WriteLine("You must be trippin, there is no enemy!");
		}
		
	}

	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void GoRoom(Command command)
	{
		
		Random rnd = new Random();
		int num = rnd.Next();
		
		Enemy currentEnemy = player.CurrentRoom.enemy;
		
		if (player.canMove == false)
		{
			Console.WriteLine("The enemy hit you in the back while running away");
			player.canMove = true;
			currentEnemy.encounterStart = false;
			player.Damage(rnd.Next(30) + 1);
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}
		
		if(!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}

		player.Damage(rnd.Next(10) + 1);
		
		Console.WriteLine("This is your current health: " + player.currentHealth());
		
		player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
		
		if (currentEnemy != null)
		{
			currentEnemy.encounterStart = true;
			player.canMove = false;
			Fight();
		}
	}
}
