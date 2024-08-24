using Godot;
using BIGConnect.addons.godotblockchain;

namespace BIGConnect.scripts;

public partial class Globals : Node
{
	public static BlockchainLogManager BlockchainLogManager { get; set; }	
	public static BlockchainClientNode BlockchainClientNode { get; set; }	
}