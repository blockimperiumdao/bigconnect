using Godot;
using System;

public partial class TestSceneSwitchScript : Node3D
{
	[Export]
	private Label addressLabel;	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PollAddress();	
	}
	
	public async void PollAddress()
	{
		var address = await BlockchainClientNode.Instance.smartWallet.GetAddress();
		GD.Print("In the new scene with address " + address );
		
		addressLabel.Text = address;

	}
	
	private void OnLoginCompleted( string address )
	{
		BlockchainManager.Instance.EmitLog("SwitchScript address " + address);
		
	}

}
