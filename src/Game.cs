using System;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using Zuul;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	
	private Enemy BossEnemy = new Enemy(200);
	private Enemy finalBoss = new Enemy(300);
	private Enemy weakEnemy1 = new Enemy(50);

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
	
	private Item gun = new Item(1, "The ultimate weapon", 500);
	
	private Item key = new Item(1, "A key to unlock a door");

	private Item goldenhat = new Item(60, "The golden hat");
	
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab", enemy: weakEnemy1);
		Room labupperfloor = new Room("on the upper floor of a computing lab");
		Room office = new Room("in the computing admin office", enemy: BossEnemy);
		Room officebasement = new Room("in the basement of the office");
		Room innerlabyrinth = new Room("in a hidden labyrinth inside the university");
		Room deeplabryrinth = new Room("in an even deeper labyrinth");
		Room chestroom = new Room("in a room with the ultimate weapon", true);
		Room treasureroom = new Room("in a room with a giant boss enemy", enemy: finalBoss);
		

		// Initialise room exits

		innerlabyrinth.AddExit("down", deeplabryrinth);
		innerlabyrinth.AddExit("north", chestroom);
		innerlabyrinth.AddExit("up",  officebasement);

		chestroom.AddExit("south", innerlabyrinth);
		chestroom.Chest.Put("gun", gun);
		
		deeplabryrinth.AddExit("up", innerlabyrinth);
		deeplabryrinth.AddExit("north", treasureroom);

		treasureroom.AddExit("south", deeplabryrinth);
		treasureroom.Chest.Put("potion", potion);
		
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);
		outside.Chest.Put("sword", sword);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);
		pub.Chest.Put("scimitar", scimitar);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);
		lab.AddExit("up", labupperfloor);
		lab.Chest.Put("key", key);

		labupperfloor.AddExit("down", lab);

		office.AddExit("west", lab);
		office.AddExit("down", officebasement);

		officebasement.AddExit("up", office);
		officebasement.AddExit("down", innerlabyrinth);
		
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
				Console.WriteLine("Yep... You're dead!");
				finished = true;
			}
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
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
		
		if (player.CurrentRoom.Chest.countItems() != 0)
		{
			Console.WriteLine("Items in room:");
			player.CurrentRoom.Chest.Show();
		}
		else
		{
			
		}
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
		if (player.CurrentRoom.enemy == BossEnemy)
		{
			Console.WriteLine("You are fighting a boss enemy, good luck!");
		} else if (player.CurrentRoom.enemy == finalBoss)
		{
			Console.WriteLine("WOW! You are in the final fight, he blocked the door, you can't run!");
		}
		else
		{
			Console.WriteLine("You are in a fight with an enemy, what do you want to do?");
			Console.WriteLine("You could run, but you will take alot of damage");
			Console.WriteLine("Or fight with the weapon you have hopefully picked up,");
			Console.WriteLine("Using attack + itemName");
		}
		
	}
	
	private void startEndSequence()
	{
		Room secretroom = new Room("in the secret room with the treasure", false);
		player.CurrentRoom.AddExit("north", secretroom);
		secretroom.Chest.Put("goldenhat", goldenhat);
		Console.WriteLine("A new room and exit has opened up!");
	}

	public void Attack(string item = null) 
	{
		Enemy currentEnemy = player.CurrentRoom.enemy;
		
		if (currentEnemy != null)
		{
			if (currentEnemy.Dead != true)
			{
								int attackPower = player.backpack.Power(item);
                				if (attackPower == 0)
                				{
                					Console.WriteLine("The enemy has not taken any damage!");
                					return;
                				}

				                if (item == "gun")
				                {
					                Console.WriteLine("You shot the enemy with a gun!");
					                Console.WriteLine("He bled out instantly!");
				                }
				                else
				                {
					                Console.WriteLine("You attacked the enemy with " + item);
				                }
                				
				                
                				currentEnemy.takeDamage(attackPower);

				                if (currentEnemy.Dead != true)
				                {
					                  Console.WriteLine("The enemy attacked as well!");
					                  Random rnd = new Random();
					                  int num = rnd.Next();
					                  player.Damage(rnd.Next(10) + 1);
					                
				                }
				                else if (currentEnemy.Dead)
				                {
					                if (currentEnemy == finalBoss)
					                {
						                startEndSequence();
					                }
					                player.canMove = true;
				                }
				              
			}
			else
			{
				Console.WriteLine("The enemy is already dead!");
				return;
			}
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
		
		if(!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		player.CurrentRoom.GetShortDescription();

		Room previousRoom = player.CurrentRoom;

		Room nextRoom = player.CurrentRoom.GetExit(direction);
		
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		} 
		
		if (nextRoom.Key == true)
		{
			if (player.backpack.Exists("key"))
			{
				player.backpack.Get("key");
				Console.WriteLine("The door has been unlocked!");
				player.CurrentRoom = nextRoom;
			} else {
				Console.WriteLine("You need a key!");
				return;
			}
		}
		
		if (player.canMove == false)
		{
			if (player.CurrentRoom.enemy == finalBoss)
			{
				Console.WriteLine("YOU CAN'T RUN! The enemy blasted you with his gun while trying to open the door!");
				player.Damage(rnd.Next(50) + 10);
				return;
			}
			Console.WriteLine("The enemy hit you in the back while running away");
			player.canMove = true;
			if (player.CurrentRoom.enemy == BossEnemy)
			{
				player.Damage(rnd.Next(50) + 10);
				player.CurrentRoom = nextRoom;
				Console.WriteLine(player.CurrentRoom.GetLongDescription());
				return;
			}
			player.Damage(rnd.Next(30) + 10);
			player.CurrentRoom = nextRoom;
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
			return;
		}
		
		
		player.Damage(rnd.Next(10) + 1);
		
		Console.WriteLine("This is your current health: " + player.currentHealth());
		
		player.CurrentRoom = nextRoom;
		
		Enemy currentEnemy = player.CurrentRoom.enemy;
		
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
		
		Console.WriteLine("Items in room:");
		Console.WriteLine(player.CurrentRoom.Chest.Show());
		
		if (currentEnemy != null && currentEnemy.Dead != true)
		{
			player.canMove = false;
			Fight();
		}
		
	}
}
