using Godot;
using System;

public partial class Connect : Node2D
{	
	private bool isAwaitingOTP = false;
	private bool isShowingInstructions = false;

	[Export]
	private Button actionButton;
	
	[Export]
	private RichTextLabel instructionsLabel;
	
	[Export]
	private PackedScene successScene;

	[Export]
	private LineEdit emailEntry;
	
	[Export]
	private LineEdit otpEntry;
	
	[Signal]
	public delegate void LoginSuccessfulEventHandler( string smartWalletAddress );
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlockchainManager.Instance.EmitLog("Connect Ready");

		actionButton.Text = "CONNECT";
		instructionsLabel.Text = "Enter your email address. \nAn account and wallet will be created for you.";
	
		BlockchainClientNode.Instance.AwaitingOTP += SetStateAwaitingOTP;
		//BlockchainClientNode.Instance.InAppWalletCreated += SetStateInAppWalletCreated;
		//BlockchainClientNode.Instance.SmartWalletCreated += SmartWalletCreated;

		//LoginSuccessful += OnLoginSuccessful;	
	}	
	
	private void SetStateAwaitingOTP()
	{		
		isAwaitingOTP = true;
		BlockchainManager.Instance.EmitLog("Awaiting OTP");

		
		actionButton.Text = "SUBMIT OTP";
		instructionsLabel.Text = "A One Time Password (OTP) has been sent to this email address. Enter it here.";

		otpEntry.Visible = true;
		emailEntry.Visible = false;

	}

	private void SetStateInAppWalletCreated( string address )
	{
		BlockchainManager.Instance.EmitLog("InAppWalletAddress " + address);
		
		otpEntry.Visible = false;
		emailEntry.Visible = false;
	}
	
	private void SmartWalletCreated( string address )
	{
		BlockchainManager.Instance.EmitLog("SmartWalletCreated " + address);

		instructionsLabel.Text = "Connected to wallet " + address;	
		actionButton.Visible = false;
		
		EmitSignal(SignalName.LoginSuccessful, address );
	}
	
	private void OnConnectButtonPressed()
	{		
		if (isAwaitingOTP == false)
		{
			BlockchainClientNode.Instance.OnStartLogin( emailEntry.Text );
		}
		else
		{
			BlockchainClientNode.Instance.OnOTPSubmit( otpEntry.Text );
		}	
	}
	
	private void OnLoginSuccessful( string smartWalletAddress )
	{	
		GD.Print("Login success");
		if (successScene != null)
		{
			GetTree().ChangeSceneToPacked(successScene);			
		}
	
	}
	
	
	
}
