/*
stores each user's data, including their name, roster, records, and inventory of items and players.
also includes methods for updating this data and saving/loading it to/from a file
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
[Serializable]
public class User
{
    public string Name { get; set; }
    public List<Player> Roster { get; set; }
    public List<Item> Inventory { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }

    public User(string name)
    {
        Name = name;
        Roster = new List<Player>();
        Inventory = new List<Item>();
        Wins = 0;
        Losses = 0;
    }

    public void AddPlayerToRoster(Player player)
    {
        Roster.Add(player);
    }

    public void AddItemToInventory(Item item)
    {
        Inventory.Add(item);
    }

    public void RecordWin()
    {
        Wins++;
    }

    public void RecordLoss()
    {
        Losses++;
    }

    public void SaveToFile(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            formatter.Serialize(stream, this);
        }
    }

    public static User LoadFromFile(string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            return (User)formatter.Deserialize(stream);
        }
    }
}