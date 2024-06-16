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
	
	[Signal]
	public delegate void LoginSuccessfulEventHandler( string smartWalletAddress );
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlockchainManager.Instance.EmitLog("Ready");

		actionButton.Text = "CONNECT";
		instructionsLabel.Text = "Enter your email address. \nAn account and wallet will be created for you.";
	
		BlockchainClientNode.Instance.AwaitingOTP += SetStateAwaitingOTP;
		BlockchainClientNode.Instance.SmartWalletCreated += SmartWalletCreated;	
	}	
	
	private void SetStateAwaitingOTP()
	{		
		isAwaitingOTP = true;
		
		actionButton.Text = "SUBMIT OTP";
		instructionsLabel.Text = "A One Time Password (OTP) has been sent to this email address. Enter it here.";
	}
	
	private void SmartWalletCreated( string address )
	{
		instructionsLabel.Text = "Connected to wallet " + address;	
		actionButton.Visible = false;
		
		EmitSignal(SignalName.LoginSuccessful);
	}
	
	private void OnConnectButtonPressed()
	{		
		if (isAwaitingOTP == false)
		{
			BlockchainClientNode.Instance.OnStartLogin();
		}
		else
		{
			BlockchainClientNode.Instance.OnOTPSubmit();
		}	
	}
	
	private void OnOTPSubmitPressed()
	{
		BlockchainManager.Instance.EmitLog("Submitting OTP");
	}
	
	
	
}
