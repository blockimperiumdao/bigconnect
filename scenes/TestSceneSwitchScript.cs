using Godot;
using System;
using System.Numerics;

public partial class TestSceneSwitchScript : Node3D
{
	[Export]
	private Label addressLabel;	

	[Export]
	private Label tokenSymbol;

	[Export]
	private Label tokenBalance;

	[Export]
	private ERC20BlockchainContractNode currencyContract;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PollAddress();	

		currencyContract.ERC20BlockchainContractInitialized += OnERC20BlockchainContractInitialized;
	}
	
	public async void OnERC20BlockchainContractInitialized()
	{
		GD.Print("ERC20BlockchainContractInitialized");

		string symbol = await currencyContract.Symbol();
		BigInteger balance = await currencyContract.BalanceOf();
		BigInteger decimals = await currencyContract.Decimals();

		balance = balance / BigInteger.Pow(10, (int)decimals);

		tokenSymbol.Text = symbol;
		tokenBalance.Text = balance.ToString();
	}

	public async void PollAddress()
	{
		var address = await BlockchainClientNode.Instance.smartWallet.GetAddress();
		GD.Print("In the new scene with address " + address );
		
		addressLabel.Text = address;

	}
	
	private void OnLoginCompleted( string address )
	{
		BlockchainLogManager.Instance.EmitLog("SwitchScript address " + address);
		
	}

}
